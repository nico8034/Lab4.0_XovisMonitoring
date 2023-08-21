using Application.Experiments.DTOs;
using Application.Services.ExperimentService;
using Domain.Entities;
using MediatR;

namespace Application.Experiments.Queries;

public class GetCurrentExperimentHandler : IRequestHandler<GetCurrentExperimentQuery, ExperimentInfoDTO>
{
    private readonly IExperimentService _experimentService;

    public GetCurrentExperimentHandler(IExperimentService experimentService)
    {
        _experimentService = experimentService;
    }
    
    public async Task<ExperimentInfoDTO> Handle(GetCurrentExperimentQuery request, CancellationToken cancellationToken)
    {
        var response = new ExperimentInfoDTO()
        {
            Id = null,
            Name = "",
            StartedAt = null
        };
        
        if (_experimentService.GetCurrentExperiment() == null)
        {
            return response;
        }

        response.Id = _experimentService.GetCurrentExperiment()!.Id;
        response.Name = _experimentService.GetCurrentExperiment()!.GetExperimentName();
        response.StartedAt = _experimentService.GetCurrentExperiment()!.GetStartTime();
        return response;
    }
}