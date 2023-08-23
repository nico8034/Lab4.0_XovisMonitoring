using MediatR;

namespace Application.SavedData.Commands.DeleteExperimentData;

public sealed record DeleteExperimentHandlerCommand(string folderName) : IRequest<string>{}