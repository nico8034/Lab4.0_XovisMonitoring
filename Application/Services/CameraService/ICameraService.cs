using Application.Common;
using Domain.Entities;

namespace Application.Services.CameraService;

public interface ICameraService
{
    Task<ServiceResponse<List<CameraImageData>>> GetStereoImage();
    Task<ServiceResponse<List<CameraImageData>>> GetValidationImage();
    Task<ServiceResponse<List<PersonCountDTO>>> GetPersonCountInView();
    List<Camera> GetCameras();
    Task<ServiceResponse<List<Camera>>> RegisterCameras();
}