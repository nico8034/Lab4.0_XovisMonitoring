using Newtonsoft.Json;

namespace Domain.Entities;

public class Zone
{
    public string CameraIp { get; set; }
    public string ZoneName { get; set; }
    public int PersonCount { get; set; } = 0;
    public double TopLeft { get; set; } = 0;
    public double TopRight { get; set; } = 0;
    public double BottomLeft { get; set; } = 0;
    public double BottomRight { get; set; } = 0;
    public DateTime LastUpdate { get; set; } = DateTime.Now;
    
    public Zone(string cameraIp, string zoneName)
    {
        CameraIp = cameraIp;
        ZoneName = zoneName;
    }
    
    public Zone(string cameraIp, string zoneName, int personCount)
    {
        CameraIp = cameraIp;
        ZoneName = zoneName;
        PersonCount = personCount;
    }
    
    [JsonConstructor]
    public Zone(string cameraIp, string zoneName, int personCount,double topLeft, double topRight, double bottomLeft, double bottomRight)
    {
        CameraIp = cameraIp;
        ZoneName = zoneName;
        PersonCount = personCount;
        TopLeft = topLeft;
        TopRight = topRight;
        BottomLeft = bottomLeft;
        BottomRight = bottomRight;
    }
}