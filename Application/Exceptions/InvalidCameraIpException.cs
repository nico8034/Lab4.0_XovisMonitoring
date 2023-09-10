using System;

namespace Application.Exceptions;

public class InvalidCameraIpException : Exception
{
    public InvalidCameraIpException(string ip) : base($@"Ip: {ip} is not a valid format") {}
}