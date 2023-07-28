namespace Domain.Exceptions;

public sealed class ExperimentNotFound : NotFoundException
{
    public ExperimentNotFound(Guid experimentId)
    {
        // : base($"The experiment with identifier {experimentId} was not found");
    }
}

public class NotFoundException : Exception
{
}