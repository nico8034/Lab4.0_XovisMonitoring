using MediatR;
using Microsoft.AspNetCore.Http;


namespace Application.Cameras.Commands.AddCamerasFile;

public sealed record AddCamerasFileCommand(IFormFile file) : IRequest<List<string>> {}