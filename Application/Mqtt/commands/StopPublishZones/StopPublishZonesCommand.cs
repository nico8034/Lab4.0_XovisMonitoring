using MediatR;

namespace Application.Mqtt.commands.StopPublishZones;

public sealed record StopPublishZonesCommand : IRequest<string>{}