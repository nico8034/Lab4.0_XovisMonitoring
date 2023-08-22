namespace Application.Exceptions;

public class NoCamerasRegistered : Exception
{
    public NoCamerasRegistered() : base("There are no cameras registered") {}
}