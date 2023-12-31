using Application.Exceptions;
using Application.Services.ExperimentService;
using Application.Services.ImageProcessingService;
using Application.Services.MonitoringService;
using Application.Services.MqttService;
using MediatR;

namespace Application.Experiments.Commands.StopExperiment;

public class StopExperimentHandler : IRequestHandler<StopExperimentCommand, string>
{
    // private readonly IExperimentRepository _experimentRepository;
    private readonly IExperimentService _experimentService;
    private readonly IImageProcessingService _imageProcessingService;
    private readonly IMonitoringService _monitoringService;
    private readonly IMqttService _mqttService;

    public StopExperimentHandler(IExperimentService experimentService, IImageProcessingService imageProcessingService, IMonitoringService monitoringService, IMqttService mqttService)
    {
        _experimentService = experimentService;
        _imageProcessingService = imageProcessingService;
        _monitoringService = monitoringService;
        _mqttService = mqttService;
    }

    public async Task<string> Handle(StopExperimentCommand request, CancellationToken cancellationToken)
    {
        if (_experimentService.GetCurrentExperiment() == null)
        {
            throw new NoActiveExperimentException();
        }

        var experimentId = _experimentService.GetCurrentExperiment()!.Id;
        
        // Stop experiment
        _experimentService.StopExperiment();
        await _mqttService.StopPublishing();
        
        // Stop monitoring service
        if (_monitoringService.IsActive())
            await _monitoringService.StopMonitoringRoom();

        // Stop background processing service
        if (_imageProcessingService.IsActive())
            _imageProcessingService.StopProcessing();
        
        return $@"Experiment with ID: {experimentId} has been stopped";

    }
}