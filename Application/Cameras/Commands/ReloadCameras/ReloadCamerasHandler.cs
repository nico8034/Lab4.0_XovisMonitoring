using Application.Services.CameraService;
using MediatR;

namespace Application.Cameras.Commands.ReloadCameras;

public class ReloadCamerasHandler : IRequestHandler<ReloadCamerasCommand,string>
{
    private readonly ICameraService _cameraService;
    
    public ReloadCamerasHandler(ICameraService cameraService)
    {
        _cameraService = cameraService;
    }
    
    public async Task<string> Handle(ReloadCamerasCommand request, CancellationToken cancellationToken)
    {
        await _cameraService.RegisterCameras();
        return "Cameras have been registered";
    }
}