using Application.Common;
using Application.Monitoring.Commands.StartMonitoring;
using Application.Monitoring.Commands.StopMonitoring;
using Application.Monitoring.Queries.GetPersonCount;
using Application.Monitoring.Queries.GetZoneByName;
using Application.Monitoring.Queries.GetZones;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]

public class MonitoringController : ApiController
{
    public MonitoringController(ISender sender) : base(sender)
    {
    }
    [HttpGet("Zones")]
    public async Task<ActionResult<ServiceResponse<List<Zone>>>> Zones(CancellationToken cancellationToken)
    {
        var query = new GetZonesQuery();
        var result = await Sender.Send(query, cancellationToken);
        
        var response = new ServiceResponse<List<Zone>>();
        
        if (result.Count > 0)
        {
            response.Message = "List of zones";
            response.Data = result;
            return Ok(response);
        }

        response.Data = null;
        response.Message = "No zones";
        return BadRequest(response);
    }

    [HttpGet("Zone/{name}")]
    public async Task<ActionResult<ServiceResponse<Zone>>> Zone(string name, CancellationToken cancellationToken)
    {
        var query = new GetZoneByNameQuery(name);
        var result = await Sender.Send(query, cancellationToken);
        var response = new ServiceResponse<Zone>()
        {
            Data = result
        };

        if (response.Data != null)
            return response;

        response.Success = false;
        response.Message = "Unable to find zone";
        return response;
    }

    [HttpGet("person/countTotal")]
    public async Task<ActionResult<ServiceResponse<int>>> PersonCount(CancellationToken cancellationToken)
    {
        var query = new GetPersonCountQuery();
        var result = await Sender.Send(query, cancellationToken);
        var response = new ServiceResponse<int>
        {
            Data = result
        };

        return response;
    }

    [HttpPost("Monitoring/Start")]
    public async Task<ActionResult<ServiceResponse<bool>>> Start(CancellationToken cancellationToken)
    {
        var command = new StartMonitoringCommand();
        var result = await Sender.Send(command, cancellationToken);
        var response = new ServiceResponse<bool>()
        {
            Data = result
        };
        
        return response;
    }

    [HttpPost("Monitoring/Stop")]
    public async Task<ActionResult<ServiceResponse<bool>>> Stop(CancellationToken cancellationToken)
    {
        var command = new StopMonitoringCommand();
        var result = await Sender.Send(command, cancellationToken);
        var response = new ServiceResponse<bool>()
        {
            Data = result
        };
        
        return response;

    }
}