using MapeamentoUrbano.API.Applications;
using MapeamentoUrbano.Domain.DTOs.Base;
using MapeamentoUrbano.Domain.DTOs.Event;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace MapeamentoUrbano.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ValidationController : Controller
    {
        public readonly ValidationApp _validationApp;

        public ValidationController(ValidationApp validationApp)
        {
            _validationApp = validationApp;
        }

        [HttpPut("Validate")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<object>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseDTO<object>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ResponseDTO<object>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ResponseDTO<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseDTO<object>))]
        public ActionResult<ResponseDTO<object>> Validate([FromRoute] int status, [FromRoute] Guid eventId)
        {
            var response = _validationApp.ValidateEvent(HttpContext, status, eventId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
