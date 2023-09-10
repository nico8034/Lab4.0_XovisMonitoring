using MediatR;

namespace Application.Mqtt.commands.StartPublishZones;

public class StartPublishZonesHandler : IRequestHandler<StartPublishZonesCommand, string>
{
    public async Task<string> Handle(StartPublishZonesCommand request, CancellationToken cancellationToken)
    {
        // Logic start MQTT zone data publisher
        return "Started MQTT Zone Publisher";
    }
}