using Application.Exceptions;
using Application.Services.CameraService;
using Application.Services.MonitoringService;
using MediatR;

namespace Application.Monitoring.Commands.StartMonitoring;

public class StartMonitoringHandler : IRequestHandler<StartMonitoringCommand, string>
{
    // private readonly IExperimentRepository _experimentRepository;
    private readonly IMonitoringService _monitoringService;
    private readonly ICameraService _cameraService;

    public StartMonitoringHandler(IMonitoringService monitoringService, ICameraService cameraService)
    {
        _monitoringService = monitoringService;
        _cameraService = cameraService;
    }

    public async Task<string> Handle(StartMonitoringCommand request, CancellationToken cancellationToken)
    {
        if (_cameraService.GetCameras().Count == 0) throw new NoCamerasRegisteredException();
        if (_monitoringService.IsActive()) throw new MonitoringServiceAlreadyActiveException();
        
        // Start monitoring service
        if (!_monitoringService.IsActive()) _monitoringService.StartMonitoringRoom();

        return "Monitoring service successfully started";
    }
    
}