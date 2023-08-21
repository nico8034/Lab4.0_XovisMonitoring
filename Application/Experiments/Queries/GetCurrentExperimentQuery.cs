using Application.Experiments.DTOs;
using MediatR;

namespace Application.Experiments.Queries;

public sealed record GetCurrentExperimentQuery() : IRequest<ExperimentInfoDTO>;