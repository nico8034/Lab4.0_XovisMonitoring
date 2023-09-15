namespace Domain.Entities;

public class ZonePersonCountDTO
{
    public DateTime CalculatedTimeStamp { get; set; }
    public DateTime XovisTimeStamp { get; set; }
    public Camera? CameraReference { get; set; }
    public Zone ZoneReference { get; set; }
}