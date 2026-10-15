using MapeamentoUrbano.API.Generics;
using MapeamentoUrbano.Data.Context;
using MapeamentoUrbano.Data.UnitOfWork;
using MapeamentoUrbano.Domain.Constants;
using MapeamentoUrbano.Domain.DTOs.Base;
using MapeamentoUrbano.Domain.DTOs.Event;
using MapeamentoUrbano.Domain.Entities;
using MapeamentoUrbano.Domain.Enums;
using MapeamentoUrbano.Domain.Helpers;
using MapeamentoUrbano.Domain.Interfaces.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace MapeamentoUrbano.API.Applications
{
    public class EventApp
    {
        private readonly UserApp _userApp;
        private readonly IUnitOfWork _unitOfWork;

        public EventApp(UserApp userApp, CoreContext context)
        {
            _userApp = userApp;
            _unitOfWork = new UnitOfWork(context);
        }

        // TODO
        // Validacao
        // Calcular Pontos


        public ResponseDTO<EventDTO> Create(HttpContext httpContext, EventDTO eventDTO)
        {
            try
            {

                #region [Validation]
                List<string> validationErrors = Validate(eventDTO);
                if (validationErrors.Count > 0)
                    return new ResponseDTO<EventDTO>(statusCode: StatusCodes.Status400BadRequest, validationErrors: validationErrors);
                #endregion

                Event evt = eventDTO.MapperToEntity();
                evt.UserId = VariableConstants.MasterUserId;
                //evt.UserId = TokenHelper.GetUserId(httpContext);

                _unitOfWork.EventRepository.Add(evt);

                if (_unitOfWork.Commit() > 0)
                    return new ResponseDTO<EventDTO>(statusCode: StatusCodes.Status201Created, entity: eventDTO);

                return new ResponseDTO<EventDTO>(statusCode: StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<EventDTO>(statusCode: StatusCodes.Status500InternalServerError, message: ex.Message);
            }
        }

        public ResponseDTO<EventDTO> Update(HttpContext httpContext, EventDTO eventDTO)
        {
            try
            {
                #region [Validation]
                List<string> validationErrors = Validate(eventDTO);
                if (validationErrors.Count > 0)
                    return new ResponseDTO<EventDTO>(statusCode: StatusCodes.Status400BadRequest, validationErrors: validationErrors);
                #endregion

                Event? eventEntity = GetEventById(eventDTO.Id);
                if (eventEntity != null)
                {
                    eventEntity.MapperToEntity(eventDTO);

                    _unitOfWork.EventRepository.AddOrUpdate(eventEntity);

                    if (_unitOfWork.Commit() > 0)
                    {
                        return new ResponseDTO<EventDTO>(statusCode: StatusCodes.Status200OK, entity: eventDTO);
                    }

                    return new ResponseDTO<EventDTO>(statusCode: StatusCodes.Status400BadRequest, message: OperationMessage.STATUS_400_INVALID_PAYLOAD_INCONSISTENT);
                }


                return new ResponseDTO<EventDTO>(statusCode: StatusCodes.Status400BadRequest, message: OperationMessage.STATUS_400_INVALID_PAYLOAD_INCONSISTENT);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<EventDTO>(statusCode: StatusCodes.Status500InternalServerError, message: ex.Message);
            }
        }

        public ResponseDTO<EventDTO> Get(HttpContext httpContext, Guid id)
        {
            try
            {
                var eventEntity = GetEventById(id);
                if (eventEntity == null)
                {
                    return new ResponseDTO<EventDTO>(statusCode: StatusCodes.Status404NotFound, message: OperationMessage.STATUS_404_NOT_FOUND);
                }

                return new ResponseDTO<EventDTO>(statusCode: StatusCodes.Status200OK, entity: new EventDTO(eventEntity));
            }
            catch (Exception ex)
            {
                return new ResponseDTO<EventDTO>(statusCode: StatusCodes.Status500InternalServerError, message: ex.Message);
            }
        }

        #region [Paginated]
        public async Task<PaginatedItemsDTO<EventDTO>> GetListAsync(string? sortOrder, int pageSize, int pageIndex, HttpContext httpContext)
        {
            var user = _userApp.GetUserById(TokenHelper.GetUserId(httpContext));

            IQueryable<Event> events = null;

            if (user.RoleId.Equals(VariableConstants.MasterId))
                 events = _unitOfWork.EventRepository.GetAll();
            else
            {
                events = _unitOfWork.EventRepository.GetAll(x => x.UserId.Equals(user.Id));
            }

            #region [Order]
            Enum.TryParse(value: sortOrder, ignoreCase: true, out SortOrder filterSortOrder);

            var itemsOrderedEnumerable = filterSortOrder switch
            {
                SortOrder.Date => GenericSort.SortByDescending(events, a => a.RegistrationDate),
                SortOrder.DateDesc => GenericSort.SortByAscending(events, a => a.RegistrationDate),
                _ => GenericSort.SortByAscending(events, a => a.RegistrationDate)
            };
            #endregion

            var itemsOnPage = await GenericSort.SkipTakeAndSelectItemsAsync(itemsOrderedEnumerable, pageSize, pageIndex, item => new EventDTO(item));
            var totalItems = await events.CountAsync();

            return new PaginatedItemsDTO<EventDTO>(pageIndex, pageSize, totalItems, itemsOnPage);
        }
        #endregion


        #region [Validation]
        public List<string> Validate(EventDTO eventDTO)
        {
            List<string> validationErrors = new List<string>();
            if (string.IsNullOrEmpty(eventDTO.Description))
                validationErrors.Add("Description is required.");
            return validationErrors;
        }
        #endregion

        internal Event? GetEventById(Guid id) =>
            _unitOfWork.EventRepository.Get(x => x.Id.Equals(id));
    }
}
