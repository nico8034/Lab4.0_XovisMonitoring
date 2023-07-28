using Application.Services.CameraService;
using Domain.Entities;

namespace Application.Services.MonitoringService;

public class MonitoringService: IMonitoringService
{
    public bool isActive { get; set; } = false;
    public Room room { get; set; }
    public int pullInterval { get; set; } = 200;

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
        
        // Thread
        // var ts = new ThreadStart(RunMonitoringRoom);
        // var backgroundThread = new Thread(ts);
        // backgroundThread.Start();

        Task.Run(RunMonitoringRoom);
    }

    public async void RunMonitoringRoom()
    {
        while (isActive)
        {
            Thread.Sleep(pullInterval);
            var personCount = await _xovisService.GetPersonCountInView();
            
            // Skip
            if (personCount.Data == null) continue;
            
            // Find match & update in room model
            foreach (var countRecord in personCount.Data)
            {
                foreach (var zone in room.GetZones())
                {
                    if (zone.Value.Name.Equals(countRecord.zone.Name))
                    {
                        zone.Value.PersonCount = countRecord.zone.PersonCount;
                    }
                }
            }
        }
    }
}