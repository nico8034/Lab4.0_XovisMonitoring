using Application.Services.CameraService;
using Application.Services.ImageProcessingService;
using Application.Services.MonitoringService;
using Domain.Entities;
using Newtonsoft.Json;

namespace API;

public class StartupService : IHostedService
{
    private ICameraService _xovisCameraService;
    private IMonitoringService _monitoringService;
    private IImageProcessingService _imageProcessingService;

    public StartupService(ICameraService xovisCameraService, IMonitoringService monitoringService, IImageProcessingService imageProcessingService)
    {
        _xovisCameraService = xovisCameraService;
        _monitoringService = monitoringService;
        _imageProcessingService = imageProcessingService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var filename = Environment.CurrentDirectory + $@"/cameras.json";
        var cameras = new List<Camera>();

        try
        {
            // cameras = JsonConvert.DeserializeObject<List<Camera>>(await File.ReadAllTextAsync(filename, cancellationToken));
            await _xovisCameraService.RegisterCameras();
            cameras = _xovisCameraService.GetCameras();
            Console.WriteLine(cameras);
            _monitoringService.SetupRoom();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unable to read file");
            Console.WriteLine(ex.Message); ;
        }

        //Setup / Start the room service

        var cameraZones = new List<Zone>();
        foreach (var camera in cameras)
        {
            Console.WriteLine($@"camera Ip: {camera.Ip}");
            foreach (var zone in camera.Zones)
            {
                Console.WriteLine($@"camera zone: {zone.Name}");
                cameraZones.Add(new Zone(camera.Ip,zone.Name));
            }
        }
        _monitoringService.GetRoom().AddZone(cameraZones);

        Console.WriteLine($"Zone count: {_monitoringService.GetRoom().GetZones().Count}");
        //
        // foreach (var item in _roomService.GetRoom().GetZones())
        // {
        //     Console.WriteLine(item.Value.Id);
        //     Console.WriteLine(item.Value.Name);
        // }

        // Start the background processing worker
        // _imageProcessingService.StartProcessing();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _monitoringService.StopMonitoringRoom();
        _imageProcessingService.StopProcessing();
    }
}