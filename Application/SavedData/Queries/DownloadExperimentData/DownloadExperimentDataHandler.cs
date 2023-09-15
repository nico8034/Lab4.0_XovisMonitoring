using Application.Common;
using Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Application.SavedData.Queries.DownloadExperimentData;

public class DownloadExperimentDatahandler : IRequestHandler<DownloadExperimentDataQuery, FileStreamResult>
{
    public async Task<FileStreamResult> Handle(DownloadExperimentDataQuery request, CancellationToken cancellationToken)
    {
        
        var experimentsFolderPath = Environment.CurrentDirectory + $@"\Experiments\{request.experimentName}";
        var dir = new DirectoryInfo(experimentsFolderPath);

        if (!dir.Exists) throw new ExperimentNotFound(request.experimentName);
        

        // Temporarily zipping to a file.
        var tempZipPath = Path.GetTempFileName() + ".zip";
        ZipUtility.ZipFolder(dir.FullName, tempZipPath);

        var stream = new FileStream(tempZipPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.DeleteOnClose);

        return new FileStreamResult(stream, "application/zip")
        {
            FileDownloadName = $"{request.experimentName}.zip"
        };
    }
}