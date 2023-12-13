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
    try
    {
      var result = await _xovisCameraService.RegisterCameras();
      _cameraInfoProvider.Cameras = result.Data!;
      _monitoringService.SetupRoom();

    }
    catch (Exception ex)
    {
      Console.WriteLine("Unable to read file");
      Console.WriteLine(ex.Message); ;
    }

    //Setup / Start the room service

    Console.WriteLine("Zones from Xovis cameras:");
    foreach (var camera in _cameraInfoProvider.Cameras!)
    {
      Console.WriteLine($@"camera Ip: {camera.Ip}");
      foreach (var zone in camera.Zones!)
      {
        Console.WriteLine($@"camera zone: {zone.zone_name}");
        // cameraZones.Add(new Zone(camera.Ip,zone.ZoneName));
      }
    }

    var jsonZones = await File.ReadAllTextAsync(zonesJsonPath);
    var predefinedZones = JsonConvert.DeserializeObject<List<Zone>>(jsonZones);

    Console.WriteLine("Zones from zones.json");

    // If no zones
    if (predefinedZones == null) return;

    foreach (var zone in predefinedZones)
    {
      Console.WriteLine($"Zone_name: {zone.zone_name}");
      Console.WriteLine($"Zone_index: {zone.zone_index}");
      // Console.WriteLine("Coordinates:");
      // foreach (var point in zone.Points)
      // {
      //   System.Console.WriteLine($"Corner: {point.Corner}");
      //   System.Console.WriteLine($"X: {point.X}");
      //   System.Console.WriteLine($"Y: {point.Y}");
      // }
    }

    _monitoringService.GetRoom().AddZone(predefinedZones);

    Console.WriteLine($"Camera count: {_cameraInfoProvider.Cameras.Count}");
    Console.WriteLine($"Camera Zone count: {_cameraInfoProvider.Cameras.Sum(o => o.Zones!.Count)}");
    Console.WriteLine($"Model Zone count: {_monitoringService.GetRoom().GetZones().Count}");
  }

  public async Task StopAsync(CancellationToken cancellationToken)
  {
    await _monitoringService.StopMonitoringRoom();
    _imageProcessingService.StopProcessing();
  }
}