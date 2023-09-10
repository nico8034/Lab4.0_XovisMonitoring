using MediatR;

namespace Application.Mqtt.commands.StopPublishZones;

public class StopPublishZonesHandler : IRequestHandler<StopPublishZonesCommand, string>
{
    public async Task<string> Handle(StopPublishZonesCommand request, CancellationToken cancellationToken)
    {
        // Logic for stopping MQTT zone published
        return "MQTT Zone publisher stopped";
    }
}