using MediatR;

namespace Application.Cameras.Queries.GetCamerasPersistent;

public sealed record GetCamerasPersistentQuery : IRequest<List<string>> {}