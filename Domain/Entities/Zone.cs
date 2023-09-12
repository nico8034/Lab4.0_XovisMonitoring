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
        Id = id;
        Name = name;
    }
    
    public Zone(string id, string name, int personCount)
    {
        Id = id;
        Name = name;
        PersonCount = personCount;
    }
}