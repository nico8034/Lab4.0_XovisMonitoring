using Application.Exceptions;
using MediatR;

namespace Application.SavedData.Commands.DeleteExperimentData;

public class DeleteExperimentDataHandler : IRequestHandler<DeleteExperimentHandlerCommand,string>
{
    public async Task<string> Handle(DeleteExperimentHandlerCommand request, CancellationToken cancellationToken)
    {
        var experimentsFolderPath = Environment.CurrentDirectory + $@"\Experiments\{request.folderName}";
        var dir = new DirectoryInfo(experimentsFolderPath);

        if (!dir.Exists) throw new ExperimentNotFound(request.folderName);

        try
        {
            dir.Delete(true);
            return $"{request.folderName} has been deleted";
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }
}