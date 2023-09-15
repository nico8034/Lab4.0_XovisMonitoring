using Application.Exceptions;
using Application.Services.CameraService;
using Domain.Entities;
using MediatR;

namespace Application.Cameras.Queries.GetCameras;

public class GetCamerasHandler : IRequestHandler<GetCamerasQuery, List<Camera>>
{
    private readonly ICameraService _cameraService;
    public GetCamerasHandler(ICameraService cameraService)
    {
        _cameraService = cameraService;
    }
    public async Task<List<Camera>> Handle(GetCamerasQuery request, CancellationToken cancellationToken)
    {
        if (_cameraService.GetCameras().Count == 0) throw new NoCamerasRegisteredException();
        return _cameraService.GetCameras();
    }
}