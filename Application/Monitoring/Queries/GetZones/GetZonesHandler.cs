using Application.Services.MonitoringService;
using Domain.Entities;
using MediatR;

namespace Application.Monitoring.Queries.GetZones;

public class GetZonesHandler : IRequestHandler<GetZonesQuery, List<Zone>>
{
    
    private readonly IMonitoringService _monitoringService;
    public GetZonesHandler(IMonitoringService monitoringService)
    {
        _monitoringService = monitoringService;
    }
    
    public async Task<List<Zone>> Handle(GetZonesQuery request, CancellationToken cancellationToken)
    {
        return _monitoringService.GetRoom().GetZones().Values.ToList();
    }
}