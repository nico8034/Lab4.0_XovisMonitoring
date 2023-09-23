using Application.Services.cameraInfo;
using Application.Services.CameraService;
using Application.Services.ImageProcessingService;
using Application.Services.MonitoringService;
using Domain.Entities;
using Newtonsoft.Json;

namespace API;

public class StartupService : IHostedService
{
  private string zonesJsonPath = Environment.CurrentDirectory + $"/cameraZones.json";
  private readonly ICameraService _xovisCameraService;
  private readonly IMonitoringService _monitoringService;
  private readonly IImageProcessingService _imageProcessingService;
  private readonly CameraInfoProvider _cameraInfoProvider;

  public StartupService(ICameraService xovisCameraService, IMonitoringService monitoringService, IImageProcessingService imageProcessingService, CameraInfoProvider cameraInfoProvider)
  {
    _xovisCameraService = xovisCameraService;
    _monitoringService = monitoringService;
    _imageProcessingService = imageProcessingService;
    _cameraInfoProvider = cameraInfoProvider;
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    var cameras = new List<Camera>();

    try
    {
      await _xovisCameraService.RegisterCameras();
      _cameraInfoProvider.Cameras = _xovisCameraService.GetCameras();
      _monitoringService.SetupRoom();

    }
    catch (Exception ex)
    {
      Console.WriteLine("Unable to read file");
      Console.WriteLine(ex.Message); ;
    }

    //Setup / Start the room service

    Console.WriteLine("Zones taken from the cameras");
    foreach (var camera in cameras)
    {
      Console.WriteLine($@"camera Ip: {camera.Ip}");
      foreach (var zone in camera.Zones)
      {
        Console.WriteLine($@"camera zone: {zone.ZoneName}");
        // cameraZones.Add(new Zone(camera.Ip,zone.ZoneName));
      }
    }

    var jsonZones = await File.ReadAllTextAsync(zonesJsonPath);
    var predefinedZones = JsonConvert.DeserializeObject<List<Zone>>(jsonZones);

    Console.WriteLine("Zones loaded from the predefined zones.json");

    // If no zones
    if (predefinedZones == null) return;

    foreach (var zone in predefinedZones)
    {
      Console.WriteLine($"Camera: {zone.CameraIp}");
      Console.WriteLine($"Zone: {zone.ZoneName}");
      Console.WriteLine("Coordinates:");
      Console.WriteLine($"TopLeft: {zone.TopLeft}");
      Console.WriteLine($"TopRight: {zone.TopRight}");
      Console.WriteLine($"BottomLeft: {zone.BottomLeft}");
      Console.WriteLine($"BottomRight: {zone.BottomRight}");
    }

    _monitoringService.GetRoom().AddZone(predefinedZones);

    Console.WriteLine($"Camera count: {_xovisCameraService.GetCameras().Count}");
    Console.WriteLine($"Zone count: {_monitoringService.GetRoom().GetZones().Count}");
  }

  public async Task StopAsync(CancellationToken cancellationToken)
  {
    _monitoringService.StopMonitoringRoom();
    _imageProcessingService.StopProcessing();
  }
}