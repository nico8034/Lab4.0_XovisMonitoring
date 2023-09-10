// using Application.Common;

using Application.Common;
using Application.Mqtt.commands.StartPublishZones;
using Application.Mqtt.commands.StopPublishZones;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class MqttController : ApiController
{
    public MqttController(ISender sender) : base(sender)
    {
    }
    
    /// <summary>
    /// Start publishing zones over MQTT
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("PublishZones")]
    public async Task<ActionResult<ServiceResponse<string>>> StartPublishing(CancellationToken cancellationToken)
    {
        var command = new StartPublishZonesCommand();
        var response = new ServiceResponse<string>();

        try
        {
            var result = await Sender.Send(command, cancellationToken);
            response.Data = result;
            return Ok(response);
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
            return BadRequest(response);
        }
    }

    /// <summary>
    /// Stop publishing zones over MQTT
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("StopPublishZones")]
    public async Task<ActionResult<ServiceResponse<string>>> StopPublishing(CancellationToken cancellationToken)
    {
        var command = new StopPublishZonesCommand();
        var response = new ServiceResponse<string>();

        try
        {
            var result = await Sender.Send(command, cancellationToken);
            response.Data = result;
            return Ok(response);
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
            return BadRequest(response);
        }
    }
}