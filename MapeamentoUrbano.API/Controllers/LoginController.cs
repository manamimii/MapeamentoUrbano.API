using MapeamentoUrbano.API.Applications;
using MapeamentoUrbano.Domain.DTOs.Base;
using MapeamentoUrbano.Domain.DTOs.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Rigging.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly LoginApp _loginApp;

        public LoginController(LoginApp loginApp)
        {
            _loginApp = loginApp;
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseDTO<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseDTO<object>))]
        public IActionResult Login(LoginDTO loginModel)
        {
            var response = _loginApp.Login(loginModel);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        [Route("renewToken")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<object>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ResponseDTO<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseDTO<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseDTO<object>))]
        public IActionResult RenewToken()
        {
            var response = _loginApp.RenewToken(HttpContext);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Route("validate")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<object>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ResponseDTO<object>))]
        public IActionResult Validate(string? token)
        {
            var response = _loginApp.Validate(token);
            return StatusCode(response);
        }
    }
}
