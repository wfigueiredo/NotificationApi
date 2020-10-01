using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NotificationApi.Domain.Model;
using NotificationApi.Interfaces.Services;
using NotificationApi.Interfaces.Strategy;
using NotificationApi.Translators;

namespace NotificationApi.Controllers
{
    [Route("/api/notification/v1")]
    public class MessageController : Controller
    {
        private readonly ILogger<MessageController> _logger;
        private readonly IMessageService _service;
        private readonly PublishStrategyContext _publishStrategyContext;

        public MessageController(ILogger<MessageController> logger, 
            IMessageService service,
            PublishStrategyContext publishStrategyContext)
        {
            _logger = logger;
            _service = service;
            _publishStrategyContext = publishStrategyContext;
        }

        [HttpPost("message")]
        [Consumes("application/json")]
        public async Task<IActionResult> Publish([FromBody] MessageDto payload, [FromHeader(Name="x-app-id")] string AppId)
        {
            try
            {
                payload.Source = AppId;
                //await _service.CreateAsync(payload.ToDomain());
                await _publishStrategyContext.Apply(payload);
                return Accepted(new { status = "Message in transit" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new { message = "Bad Request", reason = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal error in Notification Api [Publish]", reason = ex.Message });
            }
        }

        [HttpGet("message/{uniqueId}")]
        [Produces("application/json")]
        public async Task<IActionResult> FIndByUniqueId([FromRoute] string uniqueId)
        {
            try
            {
                var message = await _service.FindByUniqueIdAsync(uniqueId);
                if (message == null)
                    return NotFound();

                return Ok(message.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal error in Notification Api [FIndByUniqueId]", reason = ex.Message });
            }
        }
    }
}
