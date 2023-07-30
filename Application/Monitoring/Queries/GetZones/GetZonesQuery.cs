using Domain.Entities;
using MediatR;

namespace Application.Monitoring.Queries.GetZones;

public sealed record GetZonesQuery() : IRequest<List<Zone>>;