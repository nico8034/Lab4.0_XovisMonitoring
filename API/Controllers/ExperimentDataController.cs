using Application.Common;
using Application.DataExperiments.Queries.GetExperimentsData;
using Application.Exceptions;
using Application.SavedData.Commands.DeleteExperimentData;
using Application.SavedData.Queries.DownloadExperimentData;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Experiment Data controller
/// </summary>
[ApiController]
[Route("[controller]")]

public class ExperimentDataController : ApiController
{
    public ExperimentDataController(ISender sender) : base(sender)
    {
    }
    
    /// <summary>
    /// Get a list of saved experiments and their name
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("GetList")]
    public async Task<ActionResult<ServiceResponse<List<string>>>> GetExperiments(CancellationToken cancellationToken)
    {
        var query = new GetExperimentsDataQuery();
        var response = new ServiceResponse<List<string>>();

        try
        {
            var results = await Sender.Send(query, cancellationToken);
            response.Data = results;
            return Ok(response);
        }
        catch (DirectoryNotFoundException e)
        {
            response.Success = false;
            response.Message = e.Message;
            return NotFound(response);
        }
    }
    
    /// <summary>
    /// Download a zip folder containing data from a specific experiment
    /// </summary>
    /// <param name="experimentName">Name of experiment to download i.e: "experiment_28-07-2023T13-49-14" </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("Download")]
    public async Task<ActionResult> DownloadExperiment(string experimentName, CancellationToken cancellationToken)
    {
        var query = new DownloadExperimentDataQuery(experimentName);

        try
        {
            var results = await Sender.Send(query, cancellationToken);
            return results;
        }
        catch (ExperimentNotFound e)
        {
            return NotFound(e.Message);
        }
    }
    
    /// <summary>
    /// Delete a specific experiment
    /// </summary>
    /// <param name="experimentName">Name of experiment to delete: "experiment_28-07-2023T13-49-14" </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("Delete")]
    public async Task<ActionResult<ServiceResponse<string>>> DeleteExperiment(string experimentName, CancellationToken cancellationToken)
    {
        var query = new DeleteExperimentHandlerCommand(experimentName);
        var response = new ServiceResponse<string>();

        var result = await Sender.Send(query, cancellationToken);
        response.Data = result;
        return Ok(response);
    }
}