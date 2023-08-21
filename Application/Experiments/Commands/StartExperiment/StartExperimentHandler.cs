using Application.Services.ExperimentService;
using Application.Services.ImageProcessingService;
using Application.Services.MonitoringService;
using Domain.Abstractions;
using MediatR;

namespace Application.Experiments.Commands.StartExperiment;

public class StartExperimentHandler : IRequestHandler<StartExperimentCommand, Guid>
{
   // private readonly IExperimentRepository _experimentRepository;
    private readonly IExperimentService _experimentService;
    private readonly IImageProcessingService _imageProcessingService;
    private readonly IMonitoringService _monitoringService;

    public StartExperimentHandler(IExperimentService experimentService, IImageProcessingService imageProcessingService, IMonitoringService monitoringService)
    {
        _experimentService = experimentService;
        _imageProcessingService = imageProcessingService;
        _monitoringService = monitoringService;
    }

    public async Task<Guid> Handle(StartExperimentCommand request, CancellationToken cancellationToken)
    {
        // Start background batch processing service
        if (!_imageProcessingService.IsActive())
            _imageProcessingService.StartProcessing();
       
        // Start person monitoring service
        if (!_monitoringService.IsActive())
            _monitoringService.StartMonitoringRoom();
        
        // Start experiment service
        if (request.withImages)
        {
            _experimentService.SetDataInterval(request.interval);
        } 
        
        var experimentId = _experimentService.StartExperiment(request.withImages);

        return experimentId;
    }
    
}