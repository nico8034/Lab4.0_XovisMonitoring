namespace Domain.Entities;

public class CameraImageData
{
    public byte[] imageData { get; set; }
    public string imageType { get; set; }
    public Camera cameraInfo { get; set; }
    public DateTime timestamp { get; set; }
}