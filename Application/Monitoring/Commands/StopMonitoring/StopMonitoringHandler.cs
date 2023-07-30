using Application.Services.MonitoringService;
using MediatR;

namespace Application.Monitoring.Commands.StopMonitoring;

public class StopMonitoringHandler : IRequestHandler<StopMonitoringCommand, bool>
{
    // private readonly IExperimentRepository _experimentRepository;
    private readonly IMonitoringService _monitoringService;

    public StopMonitoringHandler(IMonitoringService monitoringService)
    {
        _monitoringService = monitoringService;
    }

    public async Task<bool> Handle(StopMonitoringCommand request, CancellationToken cancellationToken)
    {
        // Stop monitoring service
        if (_monitoringService.IsActive())
        {
            _monitoringService.StopMonitoringRoom();
        }

        return !_monitoringService.IsActive();
    }
    
}