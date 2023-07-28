using Application.Common;
using Application.Experiments.Commands.StartExperiment;
using Application.Experiments.Commands.StopExperiment;
using MediatR;

namespace API.Controllers;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

  [ApiController]
  [Route("[controller]")]
  public class ExperimentController : ApiController
  {
   
    public ExperimentController(ISender sender) : base(sender)
    {
     
    }

    [HttpPost("Start/WithImages/{interval:int}")]
    public async Task<ActionResult<ServiceResponse<Guid>>> StartExperimentWithImages(CancellationToken cancellationToken, int interval)
    {
      var command = new StartExperimentCommand(true, interval);
      var result = await Sender.Send(command,cancellationToken);

      var response = new ServiceResponse<Guid>
      {
        Data = result
      };

      return Ok(response);
      
      //
      // if (!_experimentService.isRunning())
      // {
      //   // Start monitoring service
      //   if (!_labService.IsActive())
      //     _labService.StartMonitoringRoom();
      //
      //   // Start background worker
      //   if (!_imageProcessingService.IsActive())
      //     _imageProcessingService.StartProcessing();
      //
      //   _experimentService.SetDataInterval(intervalMs);
      //
      //   var withImage = true;
      //   _experimentService.StartExperiment(withImage);
      //   response.Message = $"Experiment: {_experimentService.GetCurrentExperiment().GetExperimentName()} has been started (With Images)";
      // }
      // else
      // {
      //   response.Success = false;
      //   response.Message = $"An experiment is already in progress: {_experimentService.GetCurrentExperiment().GetExperimentName()}";
      // }
      // return response;
    }

    [HttpPost("Start/NoImages")]
    public async Task<ActionResult<ServiceResponse<Guid>>> StartWithoutImages(CancellationToken cancellationToken)
    {
      
      var command = new StartExperimentCommand(false,0);
      var result = await Sender.Send(command,cancellationToken);

      var response = new ServiceResponse<Guid>
      {
        Data = result
      };

      return response;
      // var response = new ServiceResponse<string>();
      //
      // if (!_experimentService.isRunning())
      // {
      //   // Start monitoring service
      //   if (!_labService.IsActive())
      //   {
      //     _labService.SetInterval(intervalMs);
      //     _labService.StartMonitoringRoom();
      //   }
      //
      //   // Start background worker
      //   if (!_imageProcessingService.IsActive())
      //     _imageProcessingService.StartProcessing();
      //
      //   _experimentService.SetDataInterval(intervalMs);
      //
      //   var withImage = false;
      //   _experimentService.StartExperiment(withImage);
      //   response.Message = $"Experiment: {_experimentService.GetCurrentExperiment().GetExperimentName()} has been started (Without Images)";
      // }
      // else
      // {
      //   response.Success = false;
      //   response.Message = $"An experiment is already in progress: {_experimentService.GetCurrentExperiment().GetExperimentName()}";
      // }
      // return response;
    }

    [HttpPost("Stop")]
    public async Task<ActionResult<ServiceResponse<string>>> Stop(CancellationToken cancellationToken)
    {
      var command = new StopExperimentCommand();
      var result = await Sender.Send(command,cancellationToken);

      var response = new ServiceResponse<string>
      {
        Data = result,
      };

      return response;
      //
      // var response = new ServiceResponse<string>();
      //
      // if (_experimentService.isRunning())
      // {
      //   // Stop monitoring service
      //   if (_labService.IsActive())
      //     _labService.StopMonitoringRoom();
      //
      //   if (_imageProcessingService.IsActive())
      //     _imageProcessingService.StopProcessing();
      //
      //   _experimentService.StopExperiment();
      //   response.Message = $"Experiment: {_experimentService.GetCurrentExperiment().GetExperimentName()} has been stopped";
      // }
      // else
      // {
      //   response.Success = false;
      //   response.Message = "There is no experiment in progress";
      // }
      // return response;
    }

    // [HttpGet("CurrentExperiment")]
    // public async Task<ActionResult<ServiceResponse<string>>> Status()
    // {
    //   var response = new ServiceResponse<string>();
    //   response.Data = _experimentService.GetCurrentExperiment().GetExperimentName();
    //   if (response.Data != null)
    //   {
    //     return Ok(response);
    //   }
    //   else
    //   {
    //     response.Message = "Unable to get experiment";
    //     response.Success = false;
    //     return BadRequest(response);
    //   }
    // }
  }
