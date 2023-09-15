using Application.Exceptions;
using Application.Services.MonitoringService;
using MediatR;

namespace Application.Monitoring.Commands.StopMonitoring;

public class StopMonitoringHandler : IRequestHandler<StopMonitoringCommand, string>
{
    // private readonly IExperimentRepository _experimentRepository;
    private readonly IMonitoringService _monitoringService;

    public StopMonitoringHandler(IMonitoringService monitoringService)
    {
        _monitoringService = monitoringService;
    }

    public async Task<string> Handle(StopMonitoringCommand request, CancellationToken cancellationToken)
    {
        if (!_monitoringService.IsActive()) throw new MonitoringServiceNotActiveException();
        
        // Stop monitoring service
        if (_monitoringService.IsActive())
        {
            _monitoringService.StopMonitoringRoom();
        }

        return "Monitoring service was successfully stopped";
    }
}