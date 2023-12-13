using Application.Exceptions;
using Application.Experiments.DTOs;
using Application.Mqtt.Exceptions;
using Application.Services.CameraService;
using Application.Services.ExperimentService;
using Application.Services.ImageProcessingService;
using Application.Services.MonitoringService;
using Application.Services.MqttService;
using MediatR;

namespace Application.Experiments.Commands.StartExperiment;

public class StartExperimentHandler : IRequestHandler<StartExperimentCommand, ExperimentInfoDTO>
{
   // private readonly IExperimentRepository _experimentRepository;
    private readonly IExperimentService _experimentService;
    private readonly IImageProcessingService _imageProcessingService;
    private readonly IMonitoringService _monitoringService;
    private readonly ICameraService _cameraService;
    private readonly IMqttService _mqttService;

    public StartExperimentHandler(IExperimentService experimentService, IImageProcessingService imageProcessingService, IMonitoringService monitoringService, ICameraService cameraService, IMqttService mqttService)
    {
        
        _experimentService = experimentService;
        _imageProcessingService = imageProcessingService;
        _monitoringService = monitoringService;
        _cameraService = cameraService;
        _mqttService = mqttService;
    }

    public async Task<ExperimentInfoDTO> Handle(StartExperimentCommand request, CancellationToken cancellationToken)
    {
        if (_cameraService.GetCameras().Count == 0)
        {
            throw new NoCamerasRegisteredException();
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
        
        var experimentId = _experimentService.StartExperiment(request.withImages);
        await _mqttService.Connect();

        if (_mqttService.GetMqttClient() == null)
        {
            throw new MqttClientNotInstantiated();
        }

        if (!_mqttService.isConnected())
        {
            throw new MqttNotConnected();
        }
        
        _mqttService.StartPublishing();
        
        // Images
        if (request.withImages)
        {
            if (!_imageProcessingService.IsActive())
                _imageProcessingService.StartProcessing();

            if (!_monitoringService.IsActive())
                _monitoringService.StartMonitoringRoom();
        
            _experimentService.SetDataInterval(request.interval);
        }
        // No images
        else
        {
            _monitoringService.SetExperimentName(_experimentService.GetCurrentExperiment()!.GetExperimentName());
            _monitoringService.ShouldLog(true);
            if (!_monitoringService.IsActive())
                _monitoringService.StartMonitoringRoom();
        }
        
        response.Id = experimentId;
        response.Name = _experimentService.GetCurrentExperiment()!.GetExperimentName();
        response.StartedAt = _experimentService.GetCurrentExperiment()!.GetStartTime();
        return response;
    }
}