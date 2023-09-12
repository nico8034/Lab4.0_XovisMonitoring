using Domain.Entities;

namespace Application.Services.MonitoringService;

public interface IMonitoringService
{
    void SetupRoom();
    void StopMonitoringRoom();
    void StartMonitoringRoom();
    Room GetRoom();
    bool IsActive();
    void SetInterval(int intervalMs);
    void ConfigureZonesOnRoom(List<Camera> cameras);
}