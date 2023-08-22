using MediatR;

namespace Application.Cameras.Commands;

public sealed record RemoveCameraCommand(string cameraIp) : IRequest<string>{}