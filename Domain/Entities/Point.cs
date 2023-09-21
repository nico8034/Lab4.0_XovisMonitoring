using Newtonsoft.Json;

namespace Domain.Entities;

public class Point
{
    public string Corner { get; set; } = string.Empty;
    public double X { get; set; }
    public double Y { get; set; }

    [JsonConstructor]
    public Point(string corner, double x, double y)
    {
        Corner = corner;
        X = x;
        Y = y;
    }
}