namespace Application.Exceptions;

public class NoZonesRegisteredException : Exception
{
    public NoZonesRegisteredException() : base("No zones registered") {}
}