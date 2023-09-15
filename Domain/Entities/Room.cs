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
      _zones.Add(zone.ZoneName, zone);
    }
    
    public void AddZone(List<Zone> zones)
    {
      foreach (var zone in zones)
      {
        _zones.Add(zone.ZoneName,zone);
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

    // TODO rewrite logic to not use dictionary with struct
    // The struct is as follows (string cameraIp, int number of people in the zone, Datetime when this was registered,  
    public Dictionary<string, (string, int, DateTime)> GetZonePeopleCount()
    {
      var response = new Dictionary<string, (string, int, DateTime)>();
      foreach (var zone in _zones)
      {
        (string, int, DateTime) zone_count_timestamp = (zone.Value.CameraIp, zone.Value.PersonCount, zone.Value.LastUpdate);
        // Actual zone name and the struct
        response.Add(zone.Value.ZoneName, zone_count_timestamp);
      }
      return response;
    }

    public Zone? GetZone(string name)
    {
      var zonePair = _zones.FirstOrDefault(zone => zone.Value.ZoneName == name);
      return zonePair.Value;
    }
}