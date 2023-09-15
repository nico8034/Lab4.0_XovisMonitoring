namespace Domain.Entities;

public class ExperimentData
{
    // public List<CameraImageData>? StereoImages { get; set; }
    public List<CameraImageData>? ValidationImages { get; set; }
    public DateTime Timestamp { get; set; }
    public Dictionary<string, (string, int, DateTime)> Zones { get; set; }

    // Zones are Map of string, tuple 
    // Where tuple is zone name and person count
    
    public ExperimentData(List<CameraImageData> validationImages, Dictionary<string, (string, int, DateTime)> zones)
    {
        // StereoImages = stereoImages;
        ValidationImages = validationImages;
        Timestamp = DateTime.Now;
        Zones = zones;

    }
    public ExperimentData(Dictionary<string, (string, int, DateTime)> zones)
    {
        Timestamp = DateTime.Now;
        Zones = zones;
    }
}