using System.Diagnostics;
using Application.Common;
using Domain.Entities;
using Domain.Entities.JSON;
using Newtonsoft.Json;

namespace Application.Services.CameraService;

public class XovisCameraService : ICameraService
{
  private List<Camera>? Cameras { get; set; }
  private string filePath { get; set; } = Path.Combine(Environment.CurrentDirectory,@"..\Application/Services/CameraService/cameras.txt");


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
      
      await File.WriteAllLinesAsync(filePath,listOfIps);
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
      
      await File.WriteAllLinesAsync(filePath,cameraIps);
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
          await File.WriteAllLinesAsync(filePath,listOfIps);
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
    return Cameras;
  }
  public async Task<ServiceResponse<List<Camera>>> RegisterCameras()
  {
    var listOfIps = new List<string>(await File.ReadAllLinesAsync(filePath));

    foreach (var ip in listOfIps)
    {
      Console.WriteLine(ip);
    }
    var response = new ServiceResponse<List<Camera>>()
    {
      Data = new List<Camera>()
    };

      // create tasks
      var tasks = new List<Task<Camera>>();

      Console.WriteLine("Running");
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
        // If any cameras registered
        if (listOfIps != null)
        {
          foreach (var cameraIp in listOfIps)
          {
            var zones = new List<Zone>();
            // 
            async Task<Camera> Func()
            {
              // 
              var httpResponse = await httpClient.GetAsync($"{cameraIp}/api/data/live?format=json",
                HttpCompletionOption.ResponseContentRead);
            
              httpResponse.EnsureSuccessStatusCode();

              var responseBody = await httpResponse.Content.ReadAsStringAsync();

              if (responseBody != null)
              {
                var res = JsonConvert.DeserializeObject<Root>(responseBody);
                if ((res.status.code == "OK") && (res.content.element.Count > 0))
                {
                  var cameraZones = res.content.element.FindAll(e => e.datatype == "ZONE");
                  
                  // For each of the zones present on the camera
                  foreach (var zone in cameraZones)
                  {
                    var zoneName = zone.elementname;
                    var zonePersonCount = zone.livedata.value.Find(e => e.label == "count");
                    zones.Add(new Zone(cameraIp,zoneName,zonePersonCount.value));
                  }
                 
                }
              }

              return new Camera( $"Camera_{cameraIp[^2..]}",cameraIp, zones);
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
              response.Message =
                $"Unable to fetch camera information for {t.Result.Ip}";
            }
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }

      Cameras = response.Data;

      return response;

  }
    public async Task<ServiceResponse<List<CameraImageData>>> GetStereoImage()
    {
      var response = new ServiceResponse<List<CameraImageData>>();
      response.Data = new List<CameraImageData>();

      var tasks = new List<Task<CameraImageData>>();

      try
      {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization =
          new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"admin:pass"))
          );

        if (Cameras != null)
          foreach (var camera in Cameras)
          {
            async Task<CameraImageData> Func()
            {
              var cameraData = new CameraImageData
              {
                imageType = "depth",
                cameraInfo = camera
              };
              var watcher = Stopwatch.StartNew();
              var res = await httpClient.GetAsync($"{camera.Ip}/api/scene/stereo");
              watcher.Stop();
              cameraData.timestamp = DateTime.Now.AddMilliseconds(-(watcher.ElapsedMilliseconds/2));
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
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization =
          new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"admin:pass"))
          );

        if (Cameras != null)
          foreach (var camera in Cameras)
          {
            async Task<CameraImageData> Func()
            {
              var cameraData = new CameraImageData
              {
                imageType = "validation",
                cameraInfo = camera
              };
              var watcher = Stopwatch.StartNew();
              var res = await httpClient.GetAsync($"{camera.Ip}/api/validation");
              watcher.Stop();
              cameraData.timestamp = DateTime.Now.AddMilliseconds(-(watcher.ElapsedMilliseconds/2));
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

    public async Task<ServiceResponse<List<PersonCountDTO>>> GetPersonCountInView()
    {
      var response = new ServiceResponse<List<PersonCountDTO>>
      {
        Data = new List<PersonCountDTO>()
      };

      // create tasks
      var tasks = new List<Task<List<PersonCountDTO>>>();

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
        // If any cameras registered
        if (Cameras != null)
        {
          foreach (var camera in Cameras)
          {
            // 
            async Task<List<PersonCountDTO>> Func()
            {
              var listOfPersonCounts = new List<PersonCountDTO>();
              var personCountDTO = new PersonCountDTO
              {
                CameraInfo = camera
              };

              // Time the call
              var watcher = Stopwatch.StartNew();
              // 
              var httpResponse = await httpClient.GetAsync($"{camera.Ip}/api/data/live?format=json",
                HttpCompletionOption.ResponseContentRead);
              watcher.Stop();
              httpResponse.EnsureSuccessStatusCode();

              var responseBody = await httpResponse.Content.ReadAsStringAsync();

              if (responseBody != null)
              {
                var res = JsonConvert.DeserializeObject<Root>(responseBody);
                if ((res.status.code == "OK") && (res.content.element.Count > 0))
                {
                  // cameraData.timestamp = obj.sensortime.time;

                  // subtract half of the time it took to get the response to improve accuracy of timestamp
                  // for when data was collected
                  var calculatedTimestamp = DateTime.Now.AddMilliseconds(-(watcher.ElapsedMilliseconds / 2));

                  // cameraData.XovisTimeStamp = res.content.
                  personCountDTO.Timestamp = calculatedTimestamp;

                  var cameraZones = res.content.element.FindAll(e => e.datatype == "ZONE");
                  
                  // For each of the zones present on the camera
                  foreach (var zone in cameraZones)
                  {
                    var zoneName = zone.elementname;
                    var zonePersonCount = zone.livedata.value.Find(e => e.label == "count");
                    listOfPersonCounts.Add(new PersonCountDTO()
                    {
                      Timestamp = calculatedTimestamp,
                      zone = new Zone(camera.Ip,zoneName,zonePersonCount.value),
                      XovisTimeStamp = zone.livedata.time
                    });
                  }
                }
              }

              return listOfPersonCounts;
            }

            tasks.Add(Func());
          }

          await Task.WhenAll(tasks);

          foreach (var t in tasks)
          {
            await t;
            if (t.IsCompletedSuccessfully)
            {
              response.Data.AddRange(t.Result);
            }
            else
            {
              response.Success = false;
              response.Message =
                $"Unable to fetch person count";
            }

          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }

      // Console.WriteLine(response.Data.Count);
      return response;

}
    
}