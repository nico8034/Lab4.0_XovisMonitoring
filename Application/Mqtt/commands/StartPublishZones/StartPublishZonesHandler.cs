using System.Threading;
using System.Threading.Tasks;
using Application.Mqtt.Exceptions;
using Application.Services.MqttService;
using MediatR;
using MQTTnet.Client;

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
        return "Started MQTT Zone Publisher";
    }
}