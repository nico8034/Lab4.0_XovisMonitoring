using Application.Services.CameraService;
using MediatR;

namespace Application.Cameras.Commands.AddCamerasFile;

public class AddCamerasFileHandler : IRequestHandler<AddCamerasFileCommand,List<string>>
{
    private readonly ICameraService _cameraService;
    public AddCamerasFileHandler(ICameraService cameraService)
    {
        _cameraService = cameraService;
    }
    
    public async Task<List<string>> Handle(AddCamerasFileCommand request, CancellationToken cancellationToken)
    {
        var cameraIps = new List<string>();
        var file = request.file;

        try
        {
            if (file == null || file.Length == 0)
            {
                throw new FileNotFoundException();
            }

            using var reader = new StreamReader(file.OpenReadStream());
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync(cancellationToken);
                if (!string.IsNullOrEmpty(line))
                {
                    cameraIps.Add(line);
                }
            }

            await _cameraService.AddCamerasFromFile(cameraIps);

            return cameraIps;
        
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}