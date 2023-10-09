using Newtonsoft.Json;

namespace Domain.Entities;

public class Zone
{
    public string cameraIp { get; set; }
    public string zone_name { get; set; }
    public int zone_index { get; set; }
    public int personCount { get; set; } = 0;
    public List<Point> points { get; set; } = new List<Point>();
    public DateTime timeStamp { get; set; } = DateTime.Now;
    
    public Zone(string cameraIp, string zoneName)
    {
        this.cameraIp = cameraIp;
        zone_name = zoneName;
    }
    
    public Zone(string cameraIp, string zoneName, int personCount)
    {
        this.cameraIp = cameraIp;
        zone_name = zoneName;
        this.personCount = personCount;
    }
    
    [JsonConstructor]
    public Zone(string cameraIp, string zoneName, int personCount,List<Point> points,int zoneIndex)
    {
        this.cameraIp = cameraIp;
        this.zone_name = zoneName;
        this.personCount = personCount;
        this.points = points;
        this.zone_index = zoneIndex;
    }
}