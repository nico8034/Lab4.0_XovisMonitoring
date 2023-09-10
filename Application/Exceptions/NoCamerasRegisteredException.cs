using System;

namespace Application.Exceptions;

public class NoCamerasRegisteredException : Exception
{
    public NoCamerasRegisteredException() : base("There are no cameras available") {}
}