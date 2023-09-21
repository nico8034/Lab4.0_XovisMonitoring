using Newtonsoft.Json;

namespace Domain.Entities;

public class Zone
{
    public string CameraIp { get; set; }
    public string ZoneName { get; set; }
    public int PersonCount { get; set; } = 0;
    public List<Point> Points { get; set; } = new List<Point>();
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
    public Zone(string cameraIp, string zoneName, int personCount,List<Point> points)
    {
        CameraIp = cameraIp;
        ZoneName = zoneName;
        PersonCount = personCount;
        Points = points;
    }
}