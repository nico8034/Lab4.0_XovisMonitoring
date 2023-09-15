using MediatR;

namespace Application.Cameras.Commands.ReloadCameras;

public sealed record ReloadCamerasCommand : IRequest<string> {}