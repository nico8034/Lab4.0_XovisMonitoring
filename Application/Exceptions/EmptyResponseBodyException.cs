namespace API.Exceptions;

public class EmptyResponseBodyException : Exception
{
    public EmptyResponseBodyException() : base("Response body is empty"){}


}