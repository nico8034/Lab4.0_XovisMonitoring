using Application.Services.MonitoringService;
using Domain.Entities;
using MediatR;

namespace Application.Monitoring.Queries.GetZoneByName;

public class GetZoneByNameHandler : IRequestHandler<GetZoneByNameQuery,Zone>
{
    private readonly IMonitoringService _monitoringService;
    public GetZoneByNameHandler(IMonitoringService monitoringService)
    {
        _monitoringService = monitoringService;
    }
    
    public async Task<Zone> Handle(GetZoneByNameQuery request, CancellationToken cancellationToken)
    {
        return _monitoringService.GetRoom().GetZone(request.name);
    }
}