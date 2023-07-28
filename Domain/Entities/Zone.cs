using Newtonsoft.Json;

namespace Domain.Entities;

public class Zone
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int PersonCount { get; set; } = 0;

    [JsonConstructor]
    public Zone(string id, string name)
    {
        this.Id = id;
        this.Name = name;
    }
    
    public Zone(string id, string name, int personCount)
    {
        this.Id = id;
        this.Name = name;
    }
}