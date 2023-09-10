using MediatR;

namespace Application.Mqtt.commands.StartPublishZones;

public sealed record StartPublishZonesCommand : IRequest<string> {}