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
    /// Get a list of saved experiments
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
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
    /// Download Zip folder containing specific experiment data. Use /ExperimentData to get names
    /// </summary>
    /// <param name="experimentName">Example: "experiment_28-07-2023T13-49-14" </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("Download/{experimentName}")]
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
    /// 
    /// </summary>
    /// <param name="experimentName">Example: "experiment_28-07-2023T13-49-14" </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("Delete/{experimentName}")]
    public async Task<ActionResult<ServiceResponse<string>>> DeleteExperiment(string experimentName, CancellationToken cancellationToken)
    {
        var query = new DeleteExperimentHandlerCommand(experimentName);
        var response = new ServiceResponse<string>();

        var result = await Sender.Send(query, cancellationToken);
        response.Data = result;
        return Ok(response);
    }
}