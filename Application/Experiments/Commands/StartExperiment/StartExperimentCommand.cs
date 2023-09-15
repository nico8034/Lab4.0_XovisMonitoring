using Application.Experiments.DTOs;
using MediatR;
namespace Application.Experiments.Commands.StartExperiment;


public sealed record StartExperimentCommand(bool withImages, int interval) : IRequest<ExperimentInfoDTO>;
