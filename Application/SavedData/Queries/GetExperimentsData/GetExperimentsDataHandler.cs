using MediatR;

namespace Application.DataExperiments.Queries.GetExperimentsData;

public class GetExperimentsDataHandler : IRequestHandler<GetExperimentsDataQuery,List<string>>
{
    public async Task<List<string>> Handle(GetExperimentsDataQuery request, CancellationToken cancellationToken)
    {
        // var folderPath = Environment.GetEnvironmentVariable("DOCKER_ENV") == "True"
        //     ? Path.Combine(Environment.CurrentDirectory, "Experiments")
        //     : Environment.CurrentDirectory + @"\Experiments";

        var folderPath = Path.Combine(Environment.CurrentDirectory, "Experiments");
        var dir = new DirectoryInfo(folderPath);

        if (!dir.Exists) throw new DirectoryNotFoundException();

        return dir.GetDirectories().Select(directory => directory.Name).ToList();
    }
}