using MediatR;

namespace Application.DataExperiments.Queries.GetExperimentsData;

public sealed record GetExperimentsDataQuery : IRequest<List<string>> { }