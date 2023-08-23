using System.Text;
using Application.Cameras.Commands;
using Application.Cameras.Commands.AddCamerasFile;
using Application.Cameras.Commands.ReloadCameras;
using Application.Cameras.Queries.GetCameras;
using Application.Cameras.Queries.GetCamerasPersistent;
using Application.Common;
using Application.Exceptions;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Camera controller
/// </summary>
[ApiController]
[Route("[controller]")]

public class CamerasController : ApiController
{
    public CamerasController(ISender sender) : base(sender)
    {
    }
    
    /// <summary>
    /// Get all available cameras
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpGet("Available")]
    public async Task<ActionResult<ServiceResponse<List<Camera>>>> GetCameras(CancellationToken cancellationToken)
    {
        var query = new GetCamerasQuery();
        var response = new ServiceResponse<List<Camera>>();

        try
        {
            var result = await Sender.Send(query, cancellationToken);
            response.Data = result;
            return Ok(response);
        }
        catch (NoCamerasRegisteredException e)
        {
            response.Success = false;
            response.Message = e.Message;
            return Conflict(response);
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
            return StatusCode(500, e.Message);
        }
    }
    
    /// <summary>
    /// Get list of saved cameras
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>A .txt file with all cameraIps registered in persistent storage</returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpGet("Saved")]
    public async Task<ActionResult> GetSavedCameras(CancellationToken cancellationToken)
    {
        var query = new GetCamerasPersistentQuery();

        try
        {
            var result = await Sender.Send(query,cancellationToken);
            var content = string.Join(Environment.NewLine,result);
            var byteArray = Encoding.UTF8.GetBytes(content);
            return File(byteArray, "text/plain", "cameras.txt");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    
    /// <summary>
    /// Add camera to list
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="cameraIp">Example: http://10.179.0.43</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpPost("/{cameraIp}")]
    public async Task<ActionResult<ServiceResponse<string>>> Add(string cameraIp,CancellationToken cancellationToken)
    {
        var command = new AddCameraCommand(cameraIp);
        var response = new ServiceResponse<string>();

        try
        {
            var result = await Sender.Send(command, cancellationToken);
            response.Data = result;
            return Ok(response);
        }
        catch (InvalidCameraIpException e)
        {
            response.Success = false;
            response.Message = e.Message;
            return BadRequest(response);
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
            return StatusCode(500, response);
        }
    }
    
    /// <summary>
    /// Reload available cameras from list
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="cameraIp"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpPost("Reload")]
    public async Task<ActionResult<ServiceResponse<string>>> Reload(CancellationToken cancellationToken)
    {
        var command = new ReloadCamerasCommand();
        var response = new ServiceResponse<string>();

        try
        {
            var result = await Sender.Send(command, cancellationToken);
            response.Data = result;
            response.Message = result;
            return Ok(response);
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
            return StatusCode(500, response);
        }
    }
    
    /// <summary>
    /// Upload .txt file with list of cameraIps. Check Cameras/Saved
    /// </summary>
    /// <param name="file">A .txt file with a list of cameraIps</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("File")]
    public async Task<ActionResult<ServiceResponse<List<string>>>> UploadCameraList(IFormFile file, CancellationToken cancellationToken)
    {
        var command = new AddCamerasFileCommand(file);
        var response = new ServiceResponse<List<string>>();
        
        try
        {
            var result = await Sender.Send(command, cancellationToken);
            response.Data = result;
            return Ok(response);
        }
        catch (FileNotFoundException e)
        {
            response.Success = false;
            response.Message = e.Message;
            return BadRequest(response);
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
            return StatusCode(500, e.Message);
        }
    }
    
    /// <summary>
    /// Remove camera from list
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="cameraIp">Example: http://10.179.0.43</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpDelete("Remove/{cameraIp}")]
    public async Task<ActionResult<ServiceResponse<string>>> Remove(string cameraIp,CancellationToken cancellationToken)
    {
        var command = new RemoveCameraCommand(cameraIp);
        var response = new ServiceResponse<string>();

        try
        {
            var result = await Sender.Send(command, cancellationToken);
            response.Data = result;
            return Ok(response);
        }
        catch (InvalidCameraIpException e)
        {
            response.Success = false;
            response.Message = e.Message;
            return BadRequest(response);
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
            return StatusCode(500, e.Message);
        }
    }
    
}