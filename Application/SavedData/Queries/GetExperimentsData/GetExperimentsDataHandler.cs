using MediatR;

namespace Application.DataExperiments.Queries.GetExperimentsData;

public class GetExperimentsDataHandler : IRequestHandler<GetExperimentsDataQuery,List<string>>
{
    public async Task<List<string>> Handle(GetExperimentsDataQuery request, CancellationToken cancellationToken)
    {
        var experimentsFolderPath = Environment.CurrentDirectory + @"\Experiments";
        var dir = new DirectoryInfo(experimentsFolderPath);

        if (!dir.Exists) throw new DirectoryNotFoundException();

        return dir.GetDirectories().Select(directory => directory.Name).ToList();
    }
}