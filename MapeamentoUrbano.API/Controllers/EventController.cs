using MapeamentoUrbano.API.Applications;
using MapeamentoUrbano.Domain.DTOs.Base;
using MapeamentoUrbano.Domain.DTOs.Event;
using MapeamentoUrbano.Domain.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace MapeamentoUrbano.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventController : Controller
    {
        public readonly EventApp _eventApp;

        public EventController(EventApp eventApp)
        {
            _eventApp = eventApp;
        }

        [HttpPost("Create")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseDTO<EventDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseDTO<EventDTO>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ResponseDTO<EventDTO>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ResponseDTO<EventDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseDTO<EventDTO>))]
        public ActionResult<ResponseDTO<EventDTO>> Create([FromBody] EventDTO eventDTO)
        {
            var response = _eventApp.Create(HttpContext, eventDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("Update")]
        [Authorize(Roles = "Master")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<EventDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseDTO<EventDTO>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ResponseDTO<EventDTO>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ResponseDTO<EventDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseDTO<EventDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseDTO<EventDTO>))]
        public ActionResult<ResponseDTO<EventDTO>> Update([FromBody] EventDTO eventDTO)
        {
            var response = _eventApp.Update(HttpContext, eventDTO);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get Event by Id
        /// </summary>
        /// <returns>Status code and Event</returns>
        [HttpGet("Get/{id}")]
        [Authorize(Roles = "Master")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<EventDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseDTO<EventDTO>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ResponseDTO<EventDTO>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ResponseDTO<EventDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseDTO<EventDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseDTO<EventDTO>))]
        public ActionResult<ResponseDTO<EventDTO>> Get(Guid id)
        {
            var response = _eventApp.Get(HttpContext, id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Returns a collection of Event
        /// </summary>
        /// <returns>A collection of Event</returns>
        [HttpGet("Paginated")]
        [Authorize(Roles = "Master")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<PaginatedItemsDTO<EventDTO>> Get(
            [FromQuery] string? sortOrder,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 1)
        {
            return await _eventApp.GetListAsync(sortOrder, pageSize, pageIndex, HttpContext);
        }
    }
}
