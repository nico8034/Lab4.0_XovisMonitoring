using Application.Services.CameraService;
using MediatR;

namespace Application.Cameras.Queries.GetCamerasPersistent;

public class GetCamerasPersistentHandler : IRequestHandler<GetCamerasPersistentQuery, List<string>>
{
    private readonly ICameraService _cameraService;

    public GetCamerasPersistentHandler(ICameraService cameraService)
    {
        _cameraService = cameraService;
    }
    
    public async Task<List<string>> Handle(GetCamerasPersistentQuery request, CancellationToken cancellationToken)
    {
        return await _cameraService.GetCamerasFromFile();
    }
}