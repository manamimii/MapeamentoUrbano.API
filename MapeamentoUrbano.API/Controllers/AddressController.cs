using MapeamentoUrbano.API.Applications;
using MapeamentoUrbano.Domain.DTOs.Address;
using MapeamentoUrbano.Domain.DTOs.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace MapeamentoUrbano.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AddressController : Controller
    {
        public readonly AddressApp _addressApp;

        public AddressController(AddressApp addressApp)
        {
            _addressApp = addressApp;
        }

        [HttpPost("Create")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseDTO<AddressDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseDTO<AddressDTO>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ResponseDTO<AddressDTO>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ResponseDTO<AddressDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseDTO<AddressDTO>))]
        public async Task<ActionResult<ResponseDTO<AddressDTO>>> Create([FromBody] AddressDTO addressDTO)
        {
            var response = await _addressApp.Create(HttpContext, addressDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("Update")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<AddressDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseDTO<AddressDTO>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ResponseDTO<AddressDTO>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ResponseDTO<AddressDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseDTO<AddressDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseDTO<AddressDTO>))]
        public async Task<ActionResult<ResponseDTO<AddressDTO>>> Update([FromBody] AddressDTO addressDTO)
        {
            var response = await _addressApp.Update(HttpContext, addressDTO);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get Address by Id
        /// </summary>
        /// <returns>Status code and Address</returns>
        [HttpGet("Get/{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<AddressDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseDTO<AddressDTO>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ResponseDTO<AddressDTO>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ResponseDTO<AddressDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseDTO<AddressDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseDTO<AddressDTO>))]
        public ActionResult<ResponseDTO<AddressDTO>> Get(Guid id)
        {
            var response = _addressApp.Get(HttpContext, id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Delete Address by Id
        /// </summary>
        /// <returns>Status code</returns>
        [HttpDelete("Delete/{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<object>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseDTO<object>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ResponseDTO<object>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ResponseDTO<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseDTO<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseDTO<object>))]
        public ActionResult<ResponseDTO<object>> Delete(Guid id)
        {
            var response = _addressApp.Delete(HttpContext, id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get Address by user
        /// </summary>
        /// <returns>Status code and Address</returns>
        [HttpGet("GetProfileAddress")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<AddressDTO>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ResponseDTO<AddressDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseDTO<AddressDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseDTO<AddressDTO>))]
        public ActionResult<ResponseDTO<AddressDTO>> GetProfileAddress()
        {
            var response = _addressApp.GetProfileAddress(HttpContext);
            return StatusCode(response.StatusCode, response);
        }
    }
}
