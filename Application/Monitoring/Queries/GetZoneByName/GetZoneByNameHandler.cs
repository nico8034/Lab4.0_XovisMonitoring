using Application.Exceptions;
using Application.Services.CameraService;
using Application.Services.MonitoringService;
using Domain.Entities;
using MediatR;

namespace Application.Monitoring.Queries.GetZoneByName;

public class GetZoneByNameHandler : IRequestHandler<GetZoneByNameQuery,Zone>
{
    private readonly IMonitoringService _monitoringService;
    private readonly ICameraService _cameraService;

    public GetZoneByNameHandler(IMonitoringService monitoringService, ICameraService cameraService)
    {
        _monitoringService = monitoringService;
        _cameraService = cameraService;
    }
    
    public async Task<Zone> Handle(GetZoneByNameQuery request, CancellationToken cancellationToken)
    {
        if (!_monitoringService.IsActive()) throw new MonitoringServiceNotActiveException();
        var zone = _monitoringService.GetRoom().GetZone(request.name);
        if (zone == null) throw new ZoneNotFoundException(request.name);

        return _monitoringService.GetRoom().GetZone(request.name)!;
    }
}