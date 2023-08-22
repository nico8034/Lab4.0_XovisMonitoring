namespace Application.Exceptions;

public class NoActiveExperimentException : Exception
{
    public NoActiveExperimentException() : base("There is currently no active experiment") {}
}