using System.Threading;
using System.Threading.Tasks;
using Application.Services.MqttService;
using MediatR;

namespace Application.Mqtt.commands.StartPublishZones;

public class StartPublishZonesHandler : IRequestHandler<StartPublishZonesCommand, string>
{
    private readonly IMqttService _mqttService;
    public StartPublishZonesHandler(IMqttService mqttService)
    {
        _mqttService = mqttService;
    }
    public async Task<string> Handle(StartPublishZonesCommand request, CancellationToken cancellationToken)
    {
        // Logic start MQTT zone data publisher
        await _mqttService.SetupMqttService();
        await _mqttService.PublishMessage("testTopic", "Hello World");
        return "Started MQTT Zone Publisher";
    }
}