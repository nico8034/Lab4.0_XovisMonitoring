using Application.Experiments.Commands.StartExperiment;
using Application.Services.ExperimentService;
using Application.Services.ImageProcessingService;
using Application.Services.MonitoringService;
using Domain.Abstractions;
using MediatR;

namespace Application.Experiments.Commands.StopExperiment;

public class StopExperimentHandler : IRequestHandler<StopExperimentCommand, string>
{
    // private readonly IExperimentRepository _experimentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IExperimentService _experimentService;
    private readonly IImageProcessingService _imageProcessingService;
    private readonly IMonitoringService _monitoringService;

    public StopExperimentHandler(IExperimentService experimentService, IImageProcessingService imageProcessingService, IMonitoringService monitoringService)
    {
        _experimentService = experimentService;
        _imageProcessingService = imageProcessingService;
        _monitoringService = monitoringService;
    }

    public async Task<string> Handle(StopExperimentCommand request, CancellationToken cancellationToken)
    {
        // create new experiment, assign Guid, return Guid.

        if (_experimentService.GetCurrentExperiment() == null)
        {
            return "There are no experiment to stop";
        }

        var experimentID = _experimentService.GetCurrentExperiment().Id;
        
        _experimentService.StopExperiment();
        // Stop monitoring service
        
        if (_monitoringService.IsActive())
            _monitoringService.StopMonitoringRoom();

        // Stop background processing service
        if (_imageProcessingService.IsActive())
            _imageProcessingService.StopProcessing();
        
        return $@"Experiment with ID: {experimentID} has been stopped";

    }
}