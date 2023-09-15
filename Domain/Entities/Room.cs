namespace Domain.Entities;

public class Room
{
  private readonly Dictionary<string, Zone> _zones;
  private List<Person> Persons { get; set; }

    public Room()
    {
      Persons = new List<Person>();
      _zones = new Dictionary<string, Zone>();
    }

    public void AddPerson(Person obj)
    {
      Persons.Add(obj);
    }

    public void RemovePerson(Person obj)
    {
      for (var i = 0; i < Persons.Count; i++)
      {
        if (Persons.ElementAt(i).Equals(obj))
        {
          Persons.Remove(obj);
        }
      }
    }

    public List<Person> GetPersons()
    {
      return Persons;
    }

    public void AddZone(Zone zone)
    {
      _zones.Add(zone.Name, zone);
    }
    
    public void AddZone(List<Zone> zones)
    {
      foreach (var zone in zones)
      {
        _zones.Add(zone.Name,zone);
      }
    }

    public void ClearRoom()
    {
      Persons.Clear();
    }

    public Dictionary<string, Zone> GetZones()
    {
      return _zones;
    }

    public Dictionary<string, (string, int)> GetZonePeopleCount()
    {
      var response = new Dictionary<string, (string, int)>();
      foreach (var zone in _zones)
      {
        (string, int) zoneAndPersonCount = (zone.Value.Id, zone.Value.PersonCount);
        response.Add(zone.Value.Name, zoneAndPersonCount);
      }
      return response;
    }

    public Zone? GetZone(string name)
    {
      var zonePair = _zones.FirstOrDefault(zone => zone.Value.Name == name);
      return zonePair.Value;
    }
}