using Application.Common;
using Application.Experiments.Commands.StartExperiment;
using Application.Experiments.Commands.StopExperiment;
using Application.Experiments.DTOs;
using Application.Experiments.Queries;
using MediatR;

namespace API.Controllers;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
  /// <summary>
  /// Experiment controller
  /// </summary>
  [ApiController]
  [Route("[controller]")]
  public class ExperimentController : ApiController
  {
   
    public ExperimentController(ISender sender) : base(sender)
    {
     
    }
    
    /// <summary>
    /// Start an experiment that logs and captures Stereo and Validation images
    /// </summary>
    /// <param name="interval"></param>
    /// <returns></returns>
    [HttpPost("Start/WithImages/{interval:int}")]
    public async Task<ActionResult<ServiceResponse<ExperimentInfoDTO>>> StartExperimentWithImages(CancellationToken cancellationToken, int interval)
    {
      var command = new StartExperimentCommand(true, interval);
      var result = await Sender.Send(command,cancellationToken);

      var response = new ServiceResponse<ExperimentInfoDTO>
      {
        Data = result
      };
      
      if (response.Data.Id == null)
      {
        response.Success = false;
        response.Message = "Unable to start experiment, no cameras registered";
      }

      return Ok(response);
    }
  
    /// <summary>
    /// Start an experiment that only captures logs
    /// </summary>
    /// <returns></returns>
    [HttpPost("Start/NoImages")]
    public async Task<ActionResult<ServiceResponse<ExperimentInfoDTO>>> StartWithoutImages(CancellationToken cancellationToken)
    {
      
      var command = new StartExperimentCommand(false,0);
      var result = await Sender.Send(command,cancellationToken);

      var response = new ServiceResponse<ExperimentInfoDTO>
      {
        Data = result
      };

      if (response.Data.Id == null)
      {
        response.Success = false;
        response.Message = "Unable to start experiment, no cameras registered";
      }

      return Ok(response);
    }
    
    /// <summary>
    /// Stop an ongoing experiment
    /// </summary>
    /// <returns></returns>
    [HttpPost("Stop")]
    public async Task<ActionResult<ServiceResponse<string>>> Stop(CancellationToken cancellationToken)
    {
      var command = new StopExperimentCommand();
      var result = await Sender.Send(command,cancellationToken);

      var response = new ServiceResponse<string>
      {
        Data = result,
      };
      return Ok(response);
    }
    
    /// <summary>
    /// Get information about the current experiment
    /// </summary>
    /// <returns></returns>
    [HttpGet("Current")]
    public async Task<ActionResult<ServiceResponse<ExperimentInfoDTO>>> Status(CancellationToken cancellationToken)
    {
      var query = new GetCurrentExperimentQuery();
      var result = await Sender.Send(query, cancellationToken);
      var response = new ServiceResponse<ExperimentInfoDTO>()
      {
        Data = result
      };

      return Ok(response);
    }
  }