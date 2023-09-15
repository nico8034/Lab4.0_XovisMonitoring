using Domain.Entities;
using MediatR;

namespace Application.Monitoring.Queries.GetZoneByName;

public sealed record GetZoneByNameQuery(string name) : IRequest<Zone>;