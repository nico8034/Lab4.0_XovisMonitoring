using MediatR;

namespace Application.Monitoring.Queries.GetPersonCount;

public sealed record GetPersonCountQuery() : IRequest<int>;