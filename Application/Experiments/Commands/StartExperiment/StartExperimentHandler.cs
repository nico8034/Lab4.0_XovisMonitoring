using Application.Exceptions;
using Application.Experiments.DTOs;
using Application.Services.CameraService;
using Application.Services.ExperimentService;
using Application.Services.ImageProcessingService;
using Application.Services.MonitoringService;
using MediatR;

namespace Application.Experiments.Commands.StartExperiment;

public class StartExperimentHandler : IRequestHandler<StartExperimentCommand, ExperimentInfoDTO>
{
   // private readonly IExperimentRepository _experimentRepository;
    private readonly IExperimentService _experimentService;
    private readonly IImageProcessingService _imageProcessingService;
    private readonly IMonitoringService _monitoringService;
    private readonly ICameraService _cameraService;

    public StartExperimentHandler(IExperimentService experimentService, IImageProcessingService imageProcessingService, IMonitoringService monitoringService, ICameraService cameraService)
    {
        
        _experimentService = experimentService;
        _imageProcessingService = imageProcessingService;
        _monitoringService = monitoringService;
        _cameraService = cameraService;
    }

    public async Task<ExperimentInfoDTO> Handle(StartExperimentCommand request, CancellationToken cancellationToken)
    {
        if (_cameraService.GetCameras().Count == 0)
        {
            throw new NoCamerasRegistered();
        }

        if (_experimentService.GetCurrentExperiment() != null)
        {
            throw new ExperimentAlreadyActiveException();
        }
        
        var response = new ExperimentInfoDTO()
        {
            Id = null,
            Name = "",
            StartedAt = null
        };

        // Start background batch processing service
        if (!_imageProcessingService.IsActive())
            _imageProcessingService.StartProcessing();
       
        // Start monitoring service
        if (!_monitoringService.IsActive())
            _monitoringService.StartMonitoringRoom();
        
        // Start experiment service
        if (request.withImages)
        {
            _experimentService.SetDataInterval(request.interval);
        } 
        
        var experimentId = _experimentService.StartExperiment(request.withImages);

        response.Id = experimentId;
        response.Name = _experimentService.GetCurrentExperiment()!.GetExperimentName();
        response.StartedAt = _experimentService.GetCurrentExperiment()!.GetStartTime();
        return response;
    }
    
}