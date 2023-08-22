using Domain.Entities;
using MediatR;

namespace Application.Cameras.Queries.GetCameras;

public sealed record GetCamerasQuery : IRequest<List<Camera>>{}