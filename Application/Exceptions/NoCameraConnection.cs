using System;

namespace API.Exceptions;

public class NoCameraConnection : Exception
{
    public NoCameraConnection() : base("Unable to connect to cameras"){}


}