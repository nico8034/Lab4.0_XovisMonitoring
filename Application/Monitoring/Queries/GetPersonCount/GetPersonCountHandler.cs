using Application.Exceptions;
using Application.Services.MonitoringService;
using MediatR;

namespace Application.Monitoring.Queries.GetPersonCount;

public class GetPersonCountHandler : IRequestHandler<GetPersonCountQuery, int>
{
    private readonly IMonitoringService _monitoringService;
    public GetPersonCountHandler(IMonitoringService monitoringService)
    {
        _monitoringService = monitoringService;
    }

    public async Task<int> Handle(GetPersonCountQuery request, CancellationToken cancellationToken)
    {
        if (!_monitoringService.IsActive()) throw new MonitoringServiceNotActiveException();
        return _monitoringService.GetRoom().GetZones().Sum(zone => zone.Value.PersonCount);
    }
}