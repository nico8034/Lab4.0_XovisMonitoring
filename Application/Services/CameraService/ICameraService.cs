using Application.Common;
using Domain.Entities;

namespace Application.Services.CameraService;

public interface ICameraService
{
    Task<ServiceResponse<List<CameraImageData>>> GetStereoImage();
    Task<ServiceResponse<List<CameraImageData>>> GetValidationImage();
    Task<ServiceResponse<List<ZonePersonCountDTO>>> GetPersonCountInView();
    List<Camera> GetCameras();
    Task<ServiceResponse<List<Camera>>> RegisterCameras();
    Task<List<string>> GetCamerasFromFile();
    Task<string> AddCameraToFile(string cameraIp);
    Task<string> RemoveCameraFromFile(string cameraIp);
    Task AddCamerasFromFile(List<string> cameraIps);
    
}