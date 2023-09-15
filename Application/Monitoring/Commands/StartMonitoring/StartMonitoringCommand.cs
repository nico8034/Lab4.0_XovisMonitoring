using MediatR;

namespace Application.Monitoring.Commands.StartMonitoring;

public sealed record StartMonitoringCommand() : IRequest<string>;