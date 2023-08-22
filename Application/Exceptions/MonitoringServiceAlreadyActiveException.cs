namespace Application.Exceptions;

public class MonitoringServiceAlreadyActiveException : Exception
{
    public MonitoringServiceAlreadyActiveException() : base("Monitoring service is already active") {}
}