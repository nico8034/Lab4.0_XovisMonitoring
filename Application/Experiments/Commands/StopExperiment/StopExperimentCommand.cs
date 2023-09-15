using MediatR;

namespace Application.Experiments.Commands.StopExperiment;

public sealed record StopExperimentCommand() : IRequest<string>
{
    
}