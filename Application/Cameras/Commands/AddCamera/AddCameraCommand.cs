using MediatR;

namespace Application.Cameras.Commands;

public sealed record AddCameraCommand(string cameraIp) : IRequest<string>{}