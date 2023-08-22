namespace Application.Exceptions;

public class ExperimentAlreadyActiveException : Exception
{
    public ExperimentAlreadyActiveException() : base("There is already an active experiment, stop before starting a new experiment") {}
}