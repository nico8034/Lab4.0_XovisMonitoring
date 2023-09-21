using System.Diagnostics;
using Application.Services.CameraService;
using Domain.Entities;

namespace Application.Services.MonitoringService;

public class MonitoringService: IMonitoringService
{
    public bool isActive { get; set; } = false;
    public Room room { get; set; }
    public int pullInterval { get; set; } = 50;

    private ICameraService _xovisService;
    public MonitoringService(ICameraService xovisService)
    {
        _xovisService = xovisService;
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
    }

    public void StartMonitoringRoom()
    {
        isActive = true;
        Task.Run(RunMonitoringRoom);
    }

    public async void RunMonitoringRoom()
    {
        var stopwatch = new Stopwatch();
        while (isActive)
        {
            // Thread.Sleep(pullInterval);
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
                    if (zone.Value.ZoneName.Equals(zonePersonCountDto.ZoneReference.ZoneName) && (zone.Value.CameraIp.Equals(zonePersonCountDto.ZoneReference.CameraIp)))
                    {
                        // Update the personCount for the specific zone in the room model
                        zone.Value.PersonCount = zonePersonCountDto.ZoneReference.PersonCount;
                        zone.Value.LastUpdate = zonePersonCountDto.CalculatedTimeStamp;
                    }
                }
            }
        }
    }
}