using Domain.Entities;

namespace Application.Services.MonitoringService;

public interface IMonitoringService
{
    void SetupRoom();
    Task StopMonitoringRoom();
    void StartMonitoringRoom();
    Room GetRoom();
    bool IsActive();
    void SetInterval(int intervalMs);
    void ConfigureZonesOnRoom(List<Camera> cameras);
    void ShouldLog(bool logging);
    void SetExperimentName(string experimentName);
}