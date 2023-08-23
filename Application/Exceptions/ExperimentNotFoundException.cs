namespace Application.Exceptions;

public class ExperimentNotFound : Exception
{
    public ExperimentNotFound(string experiment) : base($@"Experiment: {experiment} not found") {}
}