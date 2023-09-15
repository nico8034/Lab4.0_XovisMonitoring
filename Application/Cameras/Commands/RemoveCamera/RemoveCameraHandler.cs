using System.Text.RegularExpressions;
using System.Web;
using Application.Exceptions;
using Application.Services.CameraService;
using MediatR;

namespace Application.Cameras.Commands;

public partial class RemoveCameraHandler : IRequestHandler<RemoveCameraCommand,string>
{
    private readonly ICameraService _cameraService;

    public RemoveCameraHandler(ICameraService cameraService)
    {
        _cameraService = cameraService;
    }
    
    public async Task<string> Handle(RemoveCameraCommand request, CancellationToken cancellationToken)
    {
        
        var decodedCameraIp = HttpUtility.UrlDecode(request.cameraIp);
        var pattern = @"^http://([0-9]{1,3})\.([0-9]{1,3})\.([0-9]{1,3})\.([0-9]{1,3})$";
        var regex = new Regex(pattern);

        if (!regex.IsMatch(decodedCameraIp)) throw new InvalidCameraIpException(decodedCameraIp);

        return await _cameraService.RemoveCameraFromFile(decodedCameraIp);
    }
}