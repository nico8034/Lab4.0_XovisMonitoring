namespace Application.Exceptions;

public class MonitoringServiceNotActiveException : Exception
{
    public MonitoringServiceNotActiveException() : base("Monitoring service is not active") {}
}