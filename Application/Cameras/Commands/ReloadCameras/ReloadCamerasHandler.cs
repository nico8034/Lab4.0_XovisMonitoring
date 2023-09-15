using API.Exceptions;
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
        var result = await _cameraService.RegisterCameras();
        if (result.Data.Count == 0) throw new NoCameraConnection();
        
        return "Cameras have been registered";
    }
}