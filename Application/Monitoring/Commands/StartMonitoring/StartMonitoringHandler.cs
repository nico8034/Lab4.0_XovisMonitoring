using Application.Services.ExperimentService;
using Application.Services.ImageProcessingService;
using Application.Services.MonitoringService;
using Domain.Abstractions;
using MediatR;

namespace Application.Monitoring.Commands.StartMonitoring;

public class StartMonitoringHandler : IRequestHandler<StartMonitoringCommand, bool>
{
    // private readonly IExperimentRepository _experimentRepository;
    private readonly IMonitoringService _monitoringService;

    public StartMonitoringHandler(IMonitoringService monitoringService)
    {
        _monitoringService = monitoringService;
    }

    public async Task<bool> Handle(StartMonitoringCommand request, CancellationToken cancellationToken)
    {
        // Start monitoring service
        if (!_monitoringService.IsActive())
        {
            _monitoringService.StartMonitoringRoom();
        }

        return _monitoringService.IsActive();
    }
    
}