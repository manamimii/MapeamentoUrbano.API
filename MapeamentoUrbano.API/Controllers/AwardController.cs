using MapeamentoUrbano.API.Applications;
using MapeamentoUrbano.Domain.DTOs.Address;
using MapeamentoUrbano.Domain.DTOs.Award;
using MapeamentoUrbano.Domain.DTOs.Base;
using MapeamentoUrbano.Domain.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace MapeamentoUrbano.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AwardController : Controller
    {
        public readonly AwardApp _awardApp;

        public AwardController(AwardApp awardApp)
        {
            _awardApp = awardApp;
        }

        [HttpPost("CreateAwards")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseDTO<AddressDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseDTO<AddressDTO>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ResponseDTO<AddressDTO>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ResponseDTO<AddressDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseDTO<AddressDTO>))]
        public ActionResult<ResponseDTO<object>> CreateAwards()
        {
            var response = _awardApp.CreateAwards(HttpContext);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Returns a collection of User
        /// </summary>
        /// <returns>A collection of User</returns>
        [HttpGet("Paginated")]
        [Authorize(Roles = "Master")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<PaginatedItemsDTO<PaginatedAwardDTO>> GetListAsync(
            [FromQuery] string? sortOrder,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 1)
        {
            return await _awardApp.GetListAsync(sortOrder, pageSize, pageIndex);
        }

        /// <summary>
        /// Returns a collection of User
        /// </summary>
        /// <returns>A collection of User</returns>
        [HttpGet("GetProfileAwardsPaginated")]
        [Authorize]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public PaginatedItemsDTO<ProfilePaginatedAwardDTO> GetProfileAwardsPaginated(
            [FromQuery] string? sortOrder,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 1)
        {
            return _awardApp.GetProfileAwardsPaginated(HttpContext, sortOrder, pageSize, pageIndex);
        }
    }
}
