using Application.Common;
using Application.Exceptions;
using Application.Monitoring.Commands.StartMonitoring;
using Application.Monitoring.Commands.StopMonitoring;
using Application.Monitoring.Queries.GetPersonCount;
using Application.Monitoring.Queries.GetZoneByName;
using Application.Monitoring.Queries.GetZones;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controller for the monitoring service
/// </summary>
[ApiController]
[Route("[controller]")]

public class MonitoringController : ApiController
{
    public MonitoringController(ISender sender) : base(sender)
    {
    }
    
    /// <summary>
    /// Get data from all zones on registered cameras
    /// </summary>
    /// <returns></returns>
    [HttpGet("Zones")]
    public async Task<ActionResult<ServiceResponse<List<Zone>>>> Zones(CancellationToken cancellationToken)
    {
        var query = new GetZonesQuery();
        var response = new ServiceResponse<List<Zone>>();

        try
        {
            var result = await Sender.Send(query, cancellationToken);
            response.Data = result;
            return Ok(response);
        }
        catch (MonitoringServiceNotActiveException e)
        {
            response.Success = false;
            response.Message = e.Message;
            return Conflict(response);
        }
        catch (NoZonesRegisteredException e)
        {
            response.Success = false;
            response.Message = e.Message;
            return NotFound(response);
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
            return StatusCode(500, e.Message);
        }
    }
    
    /// <summary>
    /// Get data from specific zone by name
    /// </summary>
    /// <param name="name">The name of the specific zone, use Zones endpoint to get full list of zones</param>
    /// <returns></returns>
    [HttpGet("Zone/{name}")]
    public async Task<ActionResult<ServiceResponse<Zone>>> Zone(string name, CancellationToken cancellationToken)
    {
        var query = new GetZoneByNameQuery(name);
        var response = new ServiceResponse<Zone>();

        try
        {
            var result = await Sender.Send(query, cancellationToken);
            response.Data = result;
            return Ok(response);
        }
        catch (MonitoringServiceNotActiveException e)
        {
            response.Success = false;
            response.Message = e.Message;
            return Conflict(response);
        }
        catch (ZoneNotFoundException e)
        {
            response.Success = false;
            response.Message = e.Message;
            return NotFound(response);
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
            return StatusCode(500, e.Message);
        }
    }
    
    /// <summary>
    /// Get the total count of persons across all zones
    /// </summary>
    /// <returns></returns>
    [HttpGet("Person/Count")]
    public async Task<ActionResult<ServiceResponse<int>>> PersonCount(CancellationToken cancellationToken)
    {
        var query = new GetPersonCountQuery();
        var response = new ServiceResponse<int>();

        try
        {
            var result = await Sender.Send(query, cancellationToken);
            response.Data = result;
            return Ok(response);
        }
        catch (MonitoringServiceNotActiveException e)
        {
            response.Success = false;
            response.Message = e.Message;
            return Conflict(response);
        }
        catch (Exception e)
        {
            response.Success = false;
            return StatusCode(500, e.Message);
        }
    }
    
    /// <summary>
    /// Start the monitoring service to detect people 
    /// </summary>
    /// <returns></returns>
    [HttpPost("Start")]
    public async Task<ActionResult<ServiceResponse<string>>> Start(CancellationToken cancellationToken)
    {
        var command = new StartMonitoringCommand();
        var response = new ServiceResponse<string>();
        try
        {
            var result = await Sender.Send(command, cancellationToken);
            response.Message = result;
            return Ok(response);
        }
        catch (NoCamerasRegisteredException e)
        {
            response.Success = false;
            response.Message = e.Message;
            return Conflict(response);
        }
        catch (MonitoringServiceAlreadyActiveException e)
        {
            response.Success = false;
            response.Message = e.Message;
            return Conflict(response);
        }
        catch (Exception e)
        {
            response.Success = false;
            return StatusCode(500, e.Message);
        }
    }
    
    /// <summary>
    /// Stop the monitoring service, people will not be registered
    /// </summary>
    /// <returns></returns>
    [HttpPost("Stop")]
    public async Task<ActionResult<ServiceResponse<string>>> Stop(CancellationToken cancellationToken)
    {
        var command = new StopMonitoringCommand();
        var response = new ServiceResponse<string>();

        try
        {
            var result = await Sender.Send(command, cancellationToken);
            response.Message = result;
            return Ok(response);
        }
        catch (MonitoringServiceNotActiveException e)
        {
            response.Success = false;
            response.Message = e.Message;
            return Conflict(response);
        }
        catch (Exception e)
        {
            response.Success = false;
            return StatusCode(500, e.Message);
        }
        
    }
}