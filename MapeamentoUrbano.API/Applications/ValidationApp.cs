using MapeamentoUrbano.Data.Context;
using MapeamentoUrbano.Data.UnitOfWork;
using MapeamentoUrbano.Domain.Constants;
using MapeamentoUrbano.Domain.DTOs.Base;
using MapeamentoUrbano.Domain.DTOs.LogDTO;
using MapeamentoUrbano.Domain.Entities;
using MapeamentoUrbano.Domain.Helpers;
using MapeamentoUrbano.Domain.Interfaces.UnitOfWork;

namespace MapeamentoUrbano.API.Applications
{
    public class ValidationApp
    {
        private readonly EventApp _eventApp;
        private readonly LogApp _logApp;
        private readonly UserApp _userApp;
        private readonly IUnitOfWork _unitOfWork;

        public ValidationApp(CoreContext context, EventApp eventApp, LogApp logApp, UserApp userApp)
        {
            _unitOfWork = new UnitOfWork(context);
            _eventApp = eventApp;
            _logApp = logApp;
            _userApp = userApp;
        }

        public ResponseDTO<object> ValidateEvent(HttpContext httpContext, int status, Guid eventId)
        {
            try
            {
                Event evt = _eventApp.GetEventById(eventId);
                User user = _userApp.GetUserById(TokenHelper.GetUserId(httpContext));
                evt.Status = status;

                _unitOfWork.EventRepository.Update(evt);

                if (_unitOfWork.Commit() > 0)
                {
                    //_logApp.CreateLog(new LogDTO(ModuleConstants.Address, ActionConstants.Create, user.Id));
                    return new ResponseDTO<object>(statusCode: StatusCodes.Status200OK);
                }

                return new ResponseDTO<object>(statusCode: StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<object>(statusCode: StatusCodes.Status500InternalServerError, message: ex.Message);
            }
        }
    }
}
