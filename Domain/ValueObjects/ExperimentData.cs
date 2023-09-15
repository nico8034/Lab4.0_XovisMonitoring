namespace Domain.Entities;

public class ExperimentData
{
    public List<CameraImageData>? StereoImages { get; set; }
    public List<CameraImageData>? ValidationImages { get; set; }
    public DateTime Timestamp { get; set; }
    public Dictionary<string, (string, int)> Zones { get; set; }

    // Zones are Map of string, tuple 
    // Where tuple is zone name and person count
    
    public ExperimentData(List<CameraImageData> stereoImages, List<CameraImageData> validationImages, Dictionary<string, (string, int)> zones)
    {
        StereoImages = stereoImages;
        ValidationImages = validationImages;
        Timestamp = DateTime.Now;
        Zones = zones;

    }
    public ExperimentData(Dictionary<string, (string, int)> zones)
    {
        Timestamp = DateTime.Now;
        Zones = zones;
    }
}