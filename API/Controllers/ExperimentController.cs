using Application.Common;
using Application.Exceptions;
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
   
    public ExperimentController(ISender sender) : base(sender) {}
    
    /// <summary>
    /// Start an experiment that logs and captures Stereo and Validation images
    /// </summary>
    /// <param name="interval">Interval is in MS and must be > 100</param>
    /// <returns></returns>
    [HttpPost("Start/WithImages/{interval:int}")]
    public async Task<ActionResult<ServiceResponse<ExperimentInfoDTO>>> StartExperimentWithImages(CancellationToken cancellationToken, int interval)
    {
      var command = new StartExperimentCommand(true, interval);
      var response = new ServiceResponse<ExperimentInfoDTO> {};

      try
      {
        var result = await Sender.Send(command, cancellationToken);
        response.Data = result;
        return Ok(response);
      }
      catch (NoCamerasRegistered e)
      {
        response.Success = false;
        response.Message = e.Message;
        return UnprocessableEntity(response);
      }
      catch (ExperimentAlreadyActiveException e)
      {
        response.Success = false;
        response.Message = e.Message;
        return Conflict(response);
      }
      catch (Exception e)
      {
        response.Success = false;
        response.Message = e.Message;
        return StatusCode(500, response);
      }
    }
  
    /// <summary>
    /// Start an experiment that only captures logs
    /// </summary>
    /// <returns></returns>
    [HttpPost("Start/NoImages")]
    public async Task<ActionResult<ServiceResponse<ExperimentInfoDTO>>> StartWithoutImages(CancellationToken cancellationToken)
    {
      
      var command = new StartExperimentCommand(true, 0);
      var response = new ServiceResponse<ExperimentInfoDTO> {};

      try
      {
        var result = await Sender.Send(command, cancellationToken);
        response.Data = result;
        return Ok(response);
      }
      catch (NoCamerasRegistered e)
      {
        response.Success = false;
        response.Message = e.Message;
        return UnprocessableEntity(response);
      }
      catch (ExperimentAlreadyActiveException e)
      {
        response.Success = false;
        response.Message = e.Message;
        return Conflict(response);
      }
      catch (Exception e)
      {
        response.Success = false;
        response.Message = e.Message;
        return StatusCode(500, response);
      }
    }
    
    /// <summary>
    /// Stop an ongoing experiment
    /// </summary>
    /// <returns></returns>
    [HttpPost("Stop")]
    public async Task<ActionResult<ServiceResponse<string>>> Stop(CancellationToken cancellationToken)
    {
      var command = new StopExperimentCommand();
      var response = new ServiceResponse<string>();

      try
      {
        var result = await Sender.Send(command,cancellationToken);
        response.Data = result;
        return Ok(response);
      }
      catch (NoActiveExperimentException e)
      {
        response.Success = false;
        response.Message = e.Message;
        return NotFound(response);
      }
      catch (Exception e)
      {
        response.Success = false;
        response.Message = e.Message;
        return StatusCode(500, response);
      }
    }
    
    /// <summary>
    /// Get information about the current experiment
    /// </summary>
    /// <returns></returns>
    [HttpGet("Current")]
    public async Task<ActionResult<ServiceResponse<ExperimentInfoDTO>>> Status(CancellationToken cancellationToken)
    {
      var query = new GetCurrentExperimentQuery();
      var response = new ServiceResponse<ExperimentInfoDTO>();

      try
      {
        var result = await Sender.Send(query, cancellationToken);
        response.Data = result;
        return Ok(response);
      }
      catch (NoActiveExperimentException e)
      {
        response.Success = false;
        response.Message = e.Message;
        return NotFound(response);
      }
      catch (Exception e)
      {
        response.Success = false;
        response.Message = e.Message;
        return StatusCode(500, response);
      }
    }
  }