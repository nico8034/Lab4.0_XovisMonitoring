using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Application.SavedData.Queries.DownloadExperimentData;

public sealed record DownloadExperimentDataQuery(string experimentName) : IRequest<FileStreamResult> {}