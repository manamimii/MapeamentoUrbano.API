using MapeamentoUrbano.Data.Context;
using MapeamentoUrbano.Data.UnitOfWork;
using MapeamentoUrbano.Domain.DTOs.Base;
using MapeamentoUrbano.Domain.DTOs.Event;
using MapeamentoUrbano.Domain.Entities;
using MapeamentoUrbano.Domain.Interfaces.UnitOfWork;

namespace MapeamentoUrbano.API.Applications
{
    public class ValidationApp
    {
        private readonly EventApp _eventApp;
        private readonly IUnitOfWork _unitOfWork;

        public ValidationApp(CoreContext context, EventApp eventApp)
        {
            _unitOfWork = new UnitOfWork(context);
            _eventApp = eventApp;
        }

        public ResponseDTO<object> ValidateEvent(HttpContext httpContext, int status, Guid eventId)
        {
            try
            {
                Event evt = _eventApp.GetEventById(eventId);
                evt.Status = status;

                _unitOfWork.EventRepository.Update(evt);

                if (_unitOfWork.Commit() > 0)
                {
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
