using Application.Exceptions;
using Application.Services.CameraService;
using Application.Services.MonitoringService;
using Domain.Entities;
using MediatR;

namespace Application.Monitoring.Queries.GetZones;

public class GetZonesHandler : IRequestHandler<GetZonesQuery, List<Zone>>
{
    
    private readonly IMonitoringService _monitoringService;
    private readonly ICameraService _cameraService;
    public GetZonesHandler(IMonitoringService monitoringService, ICameraService cameraService)
    {
        _monitoringService = monitoringService;
        _cameraService = cameraService;
    }
    
    public async Task<List<Zone>> Handle(GetZonesQuery request, CancellationToken cancellationToken)
    {
        if (!_monitoringService.IsActive()) throw new MonitoringServiceNotActiveException();
        if (_monitoringService.GetRoom().GetZones().Count == 0) throw new NoZonesRegisteredException();
        return _monitoringService.GetRoom().GetZones().Values.ToList();
    }
}