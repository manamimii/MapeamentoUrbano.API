using MapeamentoUrbano.API.Applications;
using MapeamentoUrbano.Domain.DTOs.Base;
using MapeamentoUrbano.Domain.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace MapeamentoUrbano.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserApp _userApp;

        public UserController(UserApp userApp)
        {
            _userApp = userApp;
        }

        [HttpPost("Create")]
        [AllowAnonymous]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseDTO<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseDTO<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ResponseDTO<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ResponseDTO<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseDTO<UserDTO>))]
        public ActionResult<ResponseDTO<UserDTO>> Create([FromBody] UserDTO userDTO)
        {
            var response = _userApp.Create(userDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("Update")]
        [Authorize]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseDTO<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ResponseDTO<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ResponseDTO<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseDTO<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseDTO<UserDTO>))]
        public ActionResult<ResponseDTO<UserDTO>> Update([FromBody] UserDTO userDTO)
        {
            var response = _userApp.Update(HttpContext, userDTO); 
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get User by Id
        /// </summary>
        /// <returns>Status code and User</returns>
        [HttpGet("Get/{id}")]
        [Authorize]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseDTO<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ResponseDTO<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ResponseDTO<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseDTO<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseDTO<UserDTO>))]
        public ActionResult<ResponseDTO<UserDTO>> Get(Guid id)
        {
            var response = _userApp.Get(HttpContext, id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Delete User by Id
        /// </summary>
        /// <returns>Status code</returns>
        [HttpDelete("Delete/{id}")]
        [Authorize]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<object>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseDTO<object>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ResponseDTO<object>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ResponseDTO<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseDTO<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseDTO<object>))]
        public ActionResult<ResponseDTO<object>> Delete(Guid id)
        {
            var response = _userApp.Delete(HttpContext, id);
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
        public async Task<PaginatedItemsDTO<PaginatedUserDTO>> Get(
            [FromQuery] string? name,
            [FromQuery] string? sortOrder,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 1)
        {
            return await _userApp.GetListAsync(name, sortOrder, pageSize, pageIndex);
        }

        /// <summary>
        /// Returns a user profile
        /// </summary>
        /// <returns>User information</returns>
        [HttpGet("GetUserProfile")]
        [Authorize]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ResponseDTO<UserProfileDTO> GetUserProfile()
        {
            return _userApp.GetUserProfile(HttpContext);
        }

        [HttpPatch("ResetPassword/{id}")]
        [Authorize(Roles = "Master")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseDTO<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ResponseDTO<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ResponseDTO<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseDTO<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseDTO<UserDTO>))]
        public IActionResult ResetPassword(Guid id)
        {
            ResponseDTO<object> response = _userApp.ResetPassword(HttpContext, id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch("ChangePassword")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<object>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseDTO<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ResponseDTO<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseDTO<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseDTO<object>))]
        public IActionResult ChangePassword([FromQuery] string password)
        {
            ResponseDTO<object> response = _userApp.ChangePassword(HttpContext, password);
            return StatusCode(response.StatusCode, response);
        }
    }
}
