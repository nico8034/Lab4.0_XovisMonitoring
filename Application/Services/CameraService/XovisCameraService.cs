using System.Diagnostics;
using API.Exceptions;
using Application.Common;
using Application.Services.cameraInfo;
using Application.Services.Logging;
using Domain.Entities;
using Domain.Entities.JSON;
using Newtonsoft.Json;

namespace Application.Services.CameraService;

public class XovisCameraService : ICameraService
{
  private readonly HttpClient _httpClient;
  private readonly CameraInfoProvider _cameraInfoProvider;
  private readonly ILogger _logger;

  public XovisCameraService(HttpClient httpClient, CameraInfoProvider cameraInfoProvider, ILogger logger)
  {
    _httpClient = httpClient;
    _cameraInfoProvider = cameraInfoProvider;
    _logger = logger;
  }

  private string filePath
  {
    get
    {
      // Check if running inside Docker (based on an environment variable you set in the Dockerfile)
      if (Environment.GetEnvironmentVariable("DOCKER_ENV") == "True")
      {
        return Path.Combine(Environment.CurrentDirectory, "Application/Services/CameraService/cameras.txt");
      }
      return Path.Combine(Environment.CurrentDirectory, @"../Application/Services/CameraService/cameras.txt");
    }
  }

  public async Task<List<string>> GetCamerasFromFile()
  {
    return new List<string>(await File.ReadAllLinesAsync(filePath));
  }

  public async Task<string> AddCameraToFile(string cameraIp)
  {
    try
    {
      var listOfIps = new List<string>(await File.ReadAllLinesAsync(filePath));

      if (listOfIps.Contains(cameraIp)) return "Ip already exist";

      listOfIps.Add(cameraIp);

      await File.WriteAllLinesAsync(filePath, listOfIps);
      return $@"Added {cameraIp} to list";

    }
    catch (Exception e)
    {
      return e.Message;
    }
  }

  public async Task AddCamerasFromFile(List<string> cameraIps)
  {
    try
    {

      await File.WriteAllLinesAsync(filePath, cameraIps);
    }
    catch (Exception e)
    {
      Console.WriteLine(e.Message);
    }
  }

  public async Task<string> RemoveCameraFromFile(string cameraIp)
  {
    try
    {
      var listOfIps = new List<string>(await File.ReadAllLinesAsync(filePath));

      foreach (var ip in listOfIps)
      {
        if (ip.Equals(cameraIp))
        {
          listOfIps.Remove(ip);
          await File.WriteAllLinesAsync(filePath, listOfIps);
          return $@"Removed {cameraIp} from list";
        }
      }
      return $"Did not find a match in the list of cameraIps for {cameraIp}";
    }
    catch (Exception e)
    {
      return e.Message;
    }
  }
  public List<Camera> GetCameras()
  {
    return _cameraInfoProvider.Cameras;
  }
  public async Task<ServiceResponse<List<Camera>>> RegisterCameras()
  {
    var listOfIps = new List<string>(await File.ReadAllLinesAsync(filePath));

    var response = new ServiceResponse<List<Camera>>()
    {
      Data = new List<Camera>()
    };

    // create tasks
    var tasks = new List<Task<Camera>>();

    try
    {
      // Create HttpClient
      using var httpClient = new HttpClient();
      // Configure Headers
      httpClient.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue(
          "Basic",
          Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"admin:pass"))
        );

      foreach (var cameraIp in listOfIps)
      {
        var zones = new List<Zone>();

        async Task<Camera> Func()
        {
          var httpResponse = await httpClient.GetAsync($"{cameraIp}/api/data/live?format=json",
            HttpCompletionOption.ResponseContentRead);

          httpResponse.EnsureSuccessStatusCode();

          var responseBody = await httpResponse.Content.ReadAsStringAsync();

          // Response Empty
          if (responseBody == null) throw new EmptyResponseBodyException();

          var res = JsonConvert.DeserializeObject<Root>(responseBody);
          if ((res.status.code == "OK") && (res.content.element.Count > 0))
          {
            var cameraZones = res.content.element.FindAll(e => e.datatype == "ZONE");

            // For each of the zones present on the camera
            foreach (var zone in cameraZones)
            {
              var zoneName = zone.elementname;
              var zonePersonCount = zone.livedata.value.Find(e => e.label == "count");
              zones.Add(new Zone(cameraIp, zoneName, zonePersonCount.value));
            }

          }
          return new Camera($"Camera_{cameraIp[^2..]}", cameraIp, zones);
        }

        tasks.Add(Func());
      }

      await Task.WhenAll(tasks);

      foreach (var t in tasks)
      {
        await t;
        if (t.IsCompletedSuccessfully)
        {
          response.Data.Add(t.Result);
        }
        else
        {
          response.Success = false;
          response.Message = $"Unable to fetch camera information for {t.Result.Ip}";
        }
      }
    }
    catch (HttpRequestException e)
    {
      Console.WriteLine("Unable to connect to Xovis Cameras");
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
    }
    return response;

  }
  public async Task<ServiceResponse<List<CameraImageData>>> GetStereoImage()
  {
    var response = new ServiceResponse<List<CameraImageData>>();
    response.Data = new List<CameraImageData>();

    var tasks = new List<Task<CameraImageData>>();

    try
    {
      if (_cameraInfoProvider.Cameras != null)
        foreach (var camera in _cameraInfoProvider.Cameras)
        {
          async Task<CameraImageData> Func()
          {
            var cameraData = new CameraImageData
            {
              imageType = "depth",
              cameraInfo = camera
            };
            var watcher = Stopwatch.StartNew();
            var res = await _httpClient.GetAsync($"{camera.Ip}/api/scene/stereo");
            watcher.Stop();
            cameraData.timestamp = DateTime.Now.AddMilliseconds(-(watcher.ElapsedMilliseconds / 2));
            var bytes = await res.Content.ReadAsByteArrayAsync();
            cameraData.imageData = bytes;

            return cameraData;
          }

          tasks.Add(Func());
        }

      await Task.WhenAll(tasks);

      foreach (var t in tasks)
      {
        CameraImageData cameraResponse = await t;
        if (t.IsCompletedSuccessfully)
        {
          response.Data.Add(cameraResponse);
        }
        else
        {
          response.Success = false;
          response.Message = $"Unable to fetch stereo image from camera: {t.Result.cameraInfo.Name} with IP: {t.Result.cameraInfo.Ip}";
        }
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
    }

    return response;

  }
  public async Task<ServiceResponse<List<CameraImageData>>> GetValidationImage()
  {
    var response = new ServiceResponse<List<CameraImageData>>
    {
      Data = new List<CameraImageData>()
    };

    var tasks = new List<Task<CameraImageData>>();

    try
    {
      if (_cameraInfoProvider.Cameras != null)
        foreach (var camera in _cameraInfoProvider.Cameras)
        {
          async Task<CameraImageData> Func()
          {
            var cameraData = new CameraImageData
            {
              imageType = "validation",
              cameraInfo = camera
            };
            var watcher = Stopwatch.StartNew();
            var res = await _httpClient.GetAsync($"{camera.Ip}/api/validation");
            watcher.Stop();
            cameraData.timestamp = DateTime.Now.AddMilliseconds(-(watcher.ElapsedMilliseconds / 2));
            var bytes = await res.Content.ReadAsByteArrayAsync();
            cameraData.imageData = bytes;
            return cameraData;
          }

          tasks.Add(Func());
        }

      await Task.WhenAll(tasks);

      foreach (var t in tasks)
      {
        var cameraResponse = await t;
        if (t.IsCompletedSuccessfully)
        {
          response.Data.Add(cameraResponse);
        }
        else
        {
          response.Success = false;
          response.Message = $"Unable to fetch stereo image from camera: {t.Result.cameraInfo.Name} with IP: {t.Result.cameraInfo.Ip}";
        }
      }

      // Here
    }
    catch (Exception ex)
    {
      System.Console.WriteLine(ex.Message);
    }
    return response;
  }

 public async Task<ServiceResponse<List<ZonePersonCountDTO>>> GetPersonCountInView()
{
    var response = new ServiceResponse<List<ZonePersonCountDTO>>
    {
        Data = new List<ZonePersonCountDTO>()
    };

    // create tasks
    var tasks = new List<Task<List<ZonePersonCountDTO>>>();

    if (_cameraInfoProvider.Cameras != null)
    {
        foreach (var camera in _cameraInfoProvider.Cameras)
        {
            async Task<List<ZonePersonCountDTO>> Func()
            {
                var listOfPersonCounts = new List<ZonePersonCountDTO>();

                // Wrap execution of each individuel request in a try/catch
                try
                {
                    var personCountDTO = new ZonePersonCountDTO
                    {
                        CameraReference = camera
                    };

                    // Time the call
                    var watcher = Stopwatch.StartNew();
                    var httpResponse = await _httpClient.GetAsync($"{camera.Ip}/api/data/live?format=json",
                      HttpCompletionOption.ResponseContentRead);
                    watcher.Stop();
                    httpResponse.EnsureSuccessStatusCode();

                    using (var contentStream = await httpResponse.Content.ReadAsStreamAsync())
                    using (var streamReader = new StreamReader(contentStream))
                    using (var jsonReader = new JsonTextReader(streamReader))
                    {
                        var serializer = new JsonSerializer();
                        var res = serializer.Deserialize<Root>(jsonReader);

                        if ((res?.status.code == "OK") && (res.content.element.Count > 0))
                        {
                            var calculatedTimestamp = DateTime.Now.AddMilliseconds(-(watcher.ElapsedMilliseconds / 2));
                            personCountDTO.CalculatedTimeStamp = calculatedTimestamp;

                            var cameraZones = res.content.element.FindAll(e => e.datatype == "ZONE");

                            // For each of the zones present on the camera
                            foreach (var zone in cameraZones)
                            {
                                var zoneName = zone.elementname;
                                var zonePersonCount = zone.livedata.value.Find(e => e.label == "count");
                                listOfPersonCounts.Add(new ZonePersonCountDTO()
                                {
                                    CalculatedTimeStamp = calculatedTimestamp,
                                    ZoneReference = new Zone(camera.Ip, zoneName, zonePersonCount!.value),
                                    XovisTimeStamp = zone.livedata.time
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                    await _logger.WriteErrorLog($"{camera.Ip}" + ex.Message, $"{camera.Zones![0].zone_name[..7]}", "personCountErrorLog", DateTime.Now);
                }

                return listOfPersonCounts;
            }

            tasks.Add(Func());
        }
        try
        {
            var results = await Task.WhenAll(tasks);
            foreach (var result in results)
            {
                response.Data.AddRange(result);
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"An unexpected error occurred: {ex.Message}";
            await _logger.WriteErrorLog(ex.Message, "", "unexpectedErrorLog", DateTime.Now);
        }
    }
    return response;
}



}