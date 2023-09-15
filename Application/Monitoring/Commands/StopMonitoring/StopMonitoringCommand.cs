using MediatR;

namespace Application.Monitoring.Commands.StopMonitoring;

public sealed record StopMonitoringCommand() : IRequest<string>;