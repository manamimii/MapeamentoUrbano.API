using MapeamentoUrbano.API.Helper;
using MapeamentoUrbano.Data.Context;
using MapeamentoUrbano.Data.UnitOfWork;
using MapeamentoUrbano.Domain.Constants;
using MapeamentoUrbano.Domain.DTOs.Base;
using MapeamentoUrbano.Domain.DTOs.Login;
using MapeamentoUrbano.Domain.Interfaces.UnitOfWork;

namespace MapeamentoUrbano.API.Applications
{
    public class LoginApp
    {
        private readonly UserApp _userApp;
        private readonly JWTKeyApp _jwtKeyApp;
        private readonly PasswordHelper _passwordHelper;
        private readonly IUnitOfWork _unitOfWork;

        public LoginApp(UserApp userApp, JWTKeyApp jwtKeyApp, PasswordHelper passwordHelper, CoreContext context)
        {
            _userApp = userApp;
            _jwtKeyApp = jwtKeyApp;
            _passwordHelper = passwordHelper;
            _unitOfWork = new UnitOfWork(context);
        }

        #region [Login]
        internal ResponseDTO<object> Login(LoginDTO loginModel)
        {
            if (loginModel == null)
                return new ResponseDTO<object>(statusCode: StatusCodes.Status400BadRequest, message: "Dados de login não informados.");

            if (string.IsNullOrWhiteSpace(loginModel.Email) || string.IsNullOrWhiteSpace(loginModel.Password))
                return new ResponseDTO<object>(statusCode: StatusCodes.Status400BadRequest, message: "Email e senha são obrigatórios.");

            try
            {
                var user = _userApp.GetUserByLogin(loginModel.Email);
                 
                if (user == null) 
                    return new ResponseDTO<object>(StatusCodes.Status404NotFound, LoginMessage.NOT_FOUND);
                 
                if (_passwordHelper.VerifyHashedPassword(user.UserPassword, loginModel.Password))
                    return new ResponseDTO<object>(statusCode: StatusCodes.Status200OK, entity: _jwtKeyApp.GenerateUserToken(user));

                return new ResponseDTO<object>(StatusCodes.Status400BadRequest, LoginMessage.WRONG_PASSWORD);
            }
            catch (Exception ex)
            {   
                return new ResponseDTO<object>(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion

        #region [Renew Token]
        internal ResponseDTO<object> RenewToken(HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            try
            {
                string? token = httpContext.Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(token))
                    return new ResponseDTO<object>(StatusCodes.Status400BadRequest);

                return new ResponseDTO<object>(StatusCodes.Status200OK, _jwtKeyApp.RenewToken(StripBearerPrefix(token)));
            }
            catch (Exception ex)
            {
                return new ResponseDTO<object>(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion

        #region [Validate]
        internal int Validate(string? token)
        {
            if (string.IsNullOrEmpty(token))
                return StatusCodes.Status401Unauthorized;

            try
            {
                return _jwtKeyApp.ValidateToken(StripBearerPrefix(token));
            }
            catch
            {
                return StatusCodes.Status401Unauthorized;
            } 
        }
        #endregion

        #region [Helpers]
        private static string StripBearerPrefix(string token)
        {
            const string bearerPrefix = "Bearer ";
            return token.StartsWith(bearerPrefix)
                ? token[bearerPrefix.Length..]
                : token;
        }
        #endregion
    }
}
