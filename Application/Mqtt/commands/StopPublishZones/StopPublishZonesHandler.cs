using Application.Services.MqttService;
using MediatR;

namespace Application.Mqtt.commands.StopPublishZones;

public class StopPublishZonesHandler : IRequestHandler<StopPublishZonesCommand, string>
{
    private readonly IMqttService _mqttService;

    public StopPublishZonesHandler(IMqttService mqttService)
    {
        _mqttService = mqttService;
    }
    public async Task<string> Handle(StopPublishZonesCommand request, CancellationToken cancellationToken)
    {
        // Logic for stopping MQTT zone published
        // await _mqttService.PublishMessage("testTopic", "Hello World");
        await _mqttService.StopPublishing();
        return "MQTT Zone publisher stopped";
    }
}