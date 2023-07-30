namespace Domain.Entities;

public class Camera
{
    public string Ip { get; set; }
    public string Name { get; set; }
    public List<Zone>? Zones { get; set; }

    public Camera(string cameraName, string cameraIp, List<Zone> cameraZone)
    {
        Name = cameraName;
        Ip = cameraIp;
        Zones = cameraZone;
    }
}