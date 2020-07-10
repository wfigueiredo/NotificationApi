using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NotificationApi.Domain.Error;
using NotificationApi.Domain.Model;
using NotificationApi.Interfaces.Services;
using NotificationApi.Interfaces.Strategy;

namespace NotificationApi.Controllers
{
    [Route("/api/notification/v1")]
    public class PublisherController : Controller
    {
        private readonly ILogger<PublisherController> _logger;
        private readonly PublishStrategyContext _publishStrategyContext;

        public PublisherController(ILogger<PublisherController> logger, PublishStrategyContext publishStrategyContext)
        {
            _logger = logger;
            _publishStrategyContext = publishStrategyContext;
        }

        [HttpPost("publish")]
        [Consumes("application/json")]
        public async Task<IActionResult> Publish([FromBody] MessageDto payload, [FromHeader(Name="x-app-id")] string AppId)
        {
            try
            {
                payload.Source = AppId;
                await _publishStrategyContext.Apply(payload);
                return Accepted(new { status = "Message in transit" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new { message = "Bad Request", reason = ex.Message });
            }
            catch (DuplicateEntryException ex)
            {
                _logger.LogError(ex.Message);
                return Conflict(new { message = "Operation error", reason = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal error on Publisher API", reason = ex.Message });
            }
        }
    }
}
