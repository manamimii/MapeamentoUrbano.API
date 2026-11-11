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

        #region [Points]
        private int CalculatePoints(double userLatitude, double userLongitude, double photoLatitude, double photoLongitude)
        {
            const double earthRadius = 6371000;

            double latDifference = ToRadians(photoLatitude - userLatitude);

            double lonDifference = ToRadians(photoLongitude - userLongitude);

            double a = Math.Pow(Math.Sin(latDifference / 2), 2) + Math.Cos(ToRadians(userLatitude)) * Math.Cos(ToRadians(photoLatitude)) * Math.Pow(Math.Sin(lonDifference / 2), 2);

            a = Math.Clamp(a, 0.0, 1.0);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double distanceInMeters = earthRadius * c;

            return (int)Math.Round(distanceInMeters, MidpointRounding.AwayFromZero);
        }

        private static double ToRadians(double degrees) =>
            degrees * Math.PI / 180;

        #endregion

        #region [CRUD]
        public ResponseDTO<EventDTO> Create(HttpContext httpContext, EventDTO eventDTO)
        {
            try
            {
                #region [Validation]
                List<string> validationErrors = Validate(eventDTO);
                if (validationErrors.Count > 0)
                    return new ResponseDTO<EventDTO>(statusCode: StatusCodes.Status400BadRequest, validationErrors: validationErrors);
                #endregion

                Guid userId = TokenHelper.GetUserId(httpContext);

                Address? address = _unitOfWork.AddressRepository.GetAll().FirstOrDefault(x => x.UserId == userId);

                if (address == null)
                    return new ResponseDTO<EventDTO>(statusCode: StatusCodes.Status400BadRequest, message: "Usuário não possui endereço cadastrado.");

                if (address.Latitude.Equals(null) || address.Longitude.Equals(null))
                    return new ResponseDTO<EventDTO>(statusCode: StatusCodes.Status400BadRequest, message: "Endereço não possui coordenadas cadastradas.");

                Event evt = eventDTO.MapperToEntity();
                evt.UserId = userId;
                evt.Points = CalculatePoints(address.Latitude, address.Longitude, eventDTO.Latitude, eventDTO.Longitude);

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

                Guid userId = TokenHelper.GetUserId(httpContext);

                Event? eventEntity = GetEventById(eventDTO.Id);

                if (eventEntity == null)
                    return new ResponseDTO<EventDTO>(statusCode: StatusCodes.Status404NotFound, message: "Evento não encontrado.");

                if (eventEntity.UserId != userId)
                    return new ResponseDTO<EventDTO>(statusCode: StatusCodes.Status403Forbidden, message: "Você não possui permissão para alterar este evento.");

                Address? address = _unitOfWork.AddressRepository
                    .GetAll()
                    .FirstOrDefault(x => x.UserId == userId);

                if (address == null || address.Latitude.Equals(null) || address.Longitude.Equals(null))
                    return new ResponseDTO<EventDTO>(statusCode: StatusCodes.Status400BadRequest,message: "Endereço do usuário não possui coordenadas válidas.");


                eventEntity.MapperToEntity(eventDTO);

                eventEntity.UserId = userId;
                eventEntity.Points = CalculatePoints(address.Latitude,address.Longitude,eventDTO.Latitude,eventDTO.Longitude);

                _unitOfWork.EventRepository.AddOrUpdate(eventEntity);

                if (_unitOfWork.Commit() > 0)
                    return new ResponseDTO<EventDTO>(statusCode: StatusCodes.Status200OK, entity: eventDTO);

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
        #endregion 

        #region [Paginated]
        public async Task<PaginatedItemsDTO<PaginatedEventDTO>> GetListAsync(string? sortOrder, int pageSize, int pageIndex, HttpContext httpContext)
        {
            var user = _userApp.GetUserById(TokenHelper.GetUserId(httpContext));

            IQueryable<Event> events = null;

            if (user.RoleId.Equals(VariableConstants.MasterId) || user.RoleId.Equals(VariableConstants.AdminId))
                events = _unitOfWork.EventRepository.GetAll().Include(x => x.User);
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
                _ => GenericSort.SortByDescending(events, a => a.RegistrationDate)
            };
            #endregion

            var itemsOnPage = await GenericSort.SkipTakeAndSelectItemsAsync(itemsOrderedEnumerable, pageSize, pageIndex, item => new PaginatedEventDTO(item));
            var totalItems = await events.CountAsync();

            return new PaginatedItemsDTO<PaginatedEventDTO>(pageIndex, pageSize, totalItems, itemsOnPage);
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
