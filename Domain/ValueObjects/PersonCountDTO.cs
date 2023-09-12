namespace Domain.Entities;

public class PersonCountDTO
{
    public PersonCountDTO()
    {
        
    }
    
    public DateTime Timestamp { get; set; }
    public DateTime XovisTimeStamp { get; set; }
    public Camera? CameraInfo { get; set; }
    public Zone zone { get; set; }
    // public int Count { get; set; } = 0;
}