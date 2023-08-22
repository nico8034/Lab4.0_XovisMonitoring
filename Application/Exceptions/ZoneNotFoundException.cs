namespace Application.Exceptions;

public class ZoneNotFoundException : Exception
{
    public ZoneNotFoundException(string zoneName) : base($@"Zone {zoneName} not found") {}
}