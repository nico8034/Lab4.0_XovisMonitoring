using System.Diagnostics;
using Application.Services.CameraService;
using Application.Services.Logging;
using Domain.Entities;

namespace Application.Services.MonitoringService;

public class MonitoringService : IMonitoringService
{
  public bool isActive { get; set; } = false;
  public Room room { get; set; }
  public int pullInterval { get; set; } = 100;
  public bool shouldLog { get; set; } = false;
  public string ExperimentName { get; set; } = string.Empty;

  private readonly ICameraService _xovisService;
  private readonly ILogger _logger;

  public MonitoringService(ICameraService xovisService, ILogger logger)
  {
    _xovisService = xovisService;
    _logger = logger;
  }

  public void SetupRoom()
  {
    // State of room
    room = new Room();
  }

  public void ConfigureZonesOnRoom(List<Camera> cameras)
  {
    foreach (var camera in cameras)
    {
      if (camera.Zones.Count == 0) continue;
      {
        room.AddZone(camera.Zones);
      }
    }
  }

  public void ShouldLog(bool logging)
  {
    shouldLog = logging;
  }

  public void SetInterval(int intervalMs)
  {
    pullInterval = intervalMs;
  }

  public void SetRoom(Room room)
  {
    this.room = room;
  }

  public Room GetRoom()
  {
    return room;
  }

  public bool IsActive()
  {
    return isActive;
  }

  public void StopMonitoringRoom()
  {
    isActive = false;
    _logger.DisposeAsync();
  }

  public void SetExperimentName(string experimentName)
  {
    ExperimentName = experimentName;
  }

  public void StartMonitoringRoom()
  {
    isActive = true;
    Task.Run(RunMonitoringRoom);
  }

  public async Task WriteLog(ZonePersonCountDTO zone)
  {
    // Check storage path
    var experimentDataLocatiton = Environment.CurrentDirectory + $@"/Experiments/{ExperimentName}";

    // Create directory
    if (!Directory.Exists(experimentDataLocatiton))
      Directory.CreateDirectory(experimentDataLocatiton);

    // Create log file if it doesnt exist
    if (!File.Exists($"{experimentDataLocatiton}/personCountLog.txt"))
    {
      await File.WriteAllTextAsync($"{experimentDataLocatiton}/personCountLog.txt", "Date,Time logged,Updated,Zone,Person Count\n");
    }
    // Write to file if it exists
    else
    {
      await using var sw = new StreamWriter($"{experimentDataLocatiton}/personCountLog.txt", true);
      await sw.WriteLineAsync($"{zone.CalculatedTimeStamp:yyyy-MM-dd},{DateTime.Now:HH:mm:ss.fff},{zone.CalculatedTimeStamp:HH:mm:ss.fff},{zone.ZoneReference.zone_name},{zone.ZoneReference.personCount}");
    }
  }

  public async Task RunMonitoringRoom()
  {
    var stopwatch = new Stopwatch();
    while (isActive)
    {
      Thread.Sleep(pullInterval);
      stopwatch.Start();
      var result = await _xovisService.GetPersonCountInView();
      stopwatch.Stop();
      Console.WriteLine($"MONITORING: Fetched PersonCount - time in ms {stopwatch.ElapsedMilliseconds}");
      stopwatch.Reset();

      // Skip
      if (result.Data == null) continue;

      // Check each zonePersonCountDTO
      foreach (var zonePersonCountDto in result.Data)
      {
        // Check against the zones registered on the room model
        foreach (var zone in room.GetZones())
        {
          // If cameraIP and ZoneName is a match
          if (zone.Value.zone_name.Equals(zonePersonCountDto.ZoneReference.zone_name) && (zone.Value.cameraIp.Equals(zonePersonCountDto.ZoneReference.cameraIp)))
          {
            // Update the personCount for the specific zone in the room model
            zone.Value.personCount = zonePersonCountDto.ZoneReference.personCount;
            zone.Value.timeStamp = zonePersonCountDto.CalculatedTimeStamp;
          }
        }
        if (shouldLog) await _logger.WriteSuccessLog(zonePersonCountDto, ExperimentName, "personCountLog");
      }
    }
  }
}