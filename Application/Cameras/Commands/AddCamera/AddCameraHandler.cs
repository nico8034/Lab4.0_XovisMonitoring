using System.Text.RegularExpressions;
using System.Web;
using Application.Exceptions;
using Application.Services.CameraService;
using MediatR;

namespace Application.Cameras.Commands;

public partial class AddCameraHandler : IRequestHandler<AddCameraCommand,string>
{
    private readonly ICameraService _cameraService;

    public AddCameraHandler(ICameraService cameraService)
    {
        _cameraService = cameraService;
    }
    
    public async Task<string> Handle(AddCameraCommand request, CancellationToken cancellationToken)
    {
        
        var decodedCameraIp = HttpUtility.UrlDecode(request.cameraIp);
        var pattern = @"^http://([0-9]{1,3})\.([0-9]{1,3})\.([0-9]{1,3})\.([0-9]{1,3})$";
        var regex = new Regex(pattern);

        if (!regex.IsMatch(decodedCameraIp)) throw new InvalidCameraIpException(decodedCameraIp);

        return await _cameraService.AddCameraToFile(decodedCameraIp);
    }
}