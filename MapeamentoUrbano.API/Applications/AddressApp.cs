using MapeamentoUrbano.Data.Context;
using MapeamentoUrbano.Data.UnitOfWork;
using MapeamentoUrbano.Domain.Constants;
using MapeamentoUrbano.Domain.DTOs.Address;
using MapeamentoUrbano.Domain.DTOs.Base;
using MapeamentoUrbano.Domain.DTOs.LogDTO;
using MapeamentoUrbano.Domain.Entities;
using MapeamentoUrbano.Domain.Helpers;
using MapeamentoUrbano.Domain.Interfaces.UnitOfWork;
using System.Globalization;
using System.Text.Json.Serialization;

namespace MapeamentoUrbano.API.Applications
{
    public class AddressApp
    {
        private readonly HttpClient _httpClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly LogApp _logApp;
        private readonly UserApp _userApp;

        public AddressApp(HttpClient httpClient, CoreContext context, LogApp logApp, UserApp userApp)
        {
            _logApp = logApp;
            _httpClient = httpClient;
            _unitOfWork = new UnitOfWork(context);
            _userApp = userApp;
        }

        #region [Geocoding]
        private class GeocodingResult
        {
            [JsonPropertyName("lat")]
            public string Lat { get; set; } = "";

            [JsonPropertyName("lon")]
            public string Lon { get; set; } = "";
        }

        private async Task<(double Latitude, double Longitude)?> GetCoordinatesAsync(string fullAddress)
        {
            string url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(fullAddress)}&format=json&limit=1&countrycodes=br";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.UserAgent.ParseAdd("MapeamentoUrbano/1.0 (contato@seudominio.com)");

            using var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var results = await response.Content.ReadFromJsonAsync<List<GeocodingResult>>();

            if (results == null || results.Count == 0)
                return null;

            return (double.Parse(results[0].Lat, CultureInfo.InvariantCulture), double.Parse(results[0].Lon, CultureInfo.InvariantCulture));
        }
        #endregion

        #region [CRUD]
        public async Task<ResponseDTO<AddressDTO>> Create(HttpContext httpContext, AddressDTO addressDTO)
        {
            try
            {
                if (addressDTO == null)
                    return new ResponseDTO<AddressDTO>(statusCode: StatusCodes.Status400BadRequest, message: "Endereço não informado.");

                #region [Validation]
                List<string> validationErrors = Validate(addressDTO);
                if (validationErrors.Count > 0)
                    return new ResponseDTO<AddressDTO>(statusCode: StatusCodes.Status400BadRequest, validationErrors: validationErrors);
                #endregion

                User user = _userApp.GetUserById(TokenHelper.GetUserId(httpContext));

                if (user == null)
                    return new ResponseDTO<AddressDTO>(statusCode: StatusCodes.Status404NotFound, message: OperationMessage.STATUS_404_NOT_FOUND);

                Address address = addressDTO.MapperToEntity();
                address.UserId = user.Id;

                string fullAddress = $"{addressDTO.Street}, {addressDTO.Number}, {addressDTO.Neighborhood}, {addressDTO.City}, {addressDTO.State}, Brasil";
                var coordinates = await GetCoordinatesAsync(fullAddress);

                if (coordinates == null)
                    return new ResponseDTO<AddressDTO>(statusCode: StatusCodes.Status400BadRequest, message: "Não foi possível localizar o endereço informado.");

                address.Latitude = coordinates.Value.Latitude;
                address.Longitude = coordinates.Value.Longitude;

                _unitOfWork.AddressRepository.Add(address);

                user.FirstAccess = false;

                _unitOfWork.UserRepository.AddOrUpdate(user);

                if (_unitOfWork.Commit() > 0)
                {
                    addressDTO.Id = address.Id;
                    //_logApp.CreateLog(new LogDTO(ModuleConstants.Address, ActionConstants.Create, user.Id));

                    return new ResponseDTO<AddressDTO>(statusCode: StatusCodes.Status201Created, entity: addressDTO);
                }
                 
                return new ResponseDTO<AddressDTO>(statusCode: StatusCodes.Status400BadRequest, message: OperationMessage.STATUS_400_INVALID_PAYLOAD_INCONSISTENT);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<AddressDTO>(statusCode: StatusCodes.Status500InternalServerError, message: ex.Message);
            }
        }


        public async Task<ResponseDTO<AddressDTO>> Update(HttpContext httpContext, AddressDTO addressDTO)
        {
            try
            {
                if (addressDTO == null)
                    return new ResponseDTO<AddressDTO>(statusCode: StatusCodes.Status400BadRequest, message: "Endereço não informado.");

                #region [Validation]
                List<string> validationErrors = Validate(addressDTO);
                if (validationErrors.Count > 0)
                    return new ResponseDTO<AddressDTO>(statusCode: StatusCodes.Status400BadRequest, validationErrors: validationErrors);
                #endregion

                Address? address = GetAddressById(addressDTO.Id);

                if (address == null)
                    return new ResponseDTO<AddressDTO>(statusCode: StatusCodes.Status404NotFound, message: "Endereço não encontrado.");

                var userId = TokenHelper.GetUserId(httpContext);

                if (address.UserId != userId)
                    return new ResponseDTO<AddressDTO>(statusCode: StatusCodes.Status403Forbidden, message: OperationMessage.STATUS_403_FORBIDDEN);

                string fullAddress = $"{addressDTO.Street}, {addressDTO.Number}, {addressDTO.Neighborhood}, {addressDTO.City}, {addressDTO.State}, Brasil";

                var coordinates = await GetCoordinatesAsync(fullAddress);

                if (coordinates == null)
                    return new ResponseDTO<AddressDTO>(statusCode: StatusCodes.Status400BadRequest, message: "Não foi possível localizar o endereço informado.");

                address.MapperToEntity(addressDTO);

                address.Latitude = coordinates.Value.Latitude;
                address.Longitude = coordinates.Value.Longitude;

                _unitOfWork.AddressRepository.AddOrUpdate(address);

                if (_unitOfWork.Commit() > 0)
                {
                    //_logApp.CreateLog(new LogDTO(ModuleConstants.Address, ActionConstants.Update, userId));

                    return new ResponseDTO<AddressDTO>(statusCode: StatusCodes.Status200OK, entity: addressDTO);
                }

                return new ResponseDTO<AddressDTO>(statusCode: StatusCodes.Status400BadRequest, message: OperationMessage.STATUS_400_INVALID_PAYLOAD_INCONSISTENT);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<AddressDTO>(statusCode: StatusCodes.Status500InternalServerError, message: ex.Message);
            }
        }

        public ResponseDTO<AddressDTO> Get(HttpContext httpContext, Guid id)
        {
            try
            {
                var address = GetAddressById(id);
                if (address == null)
                {
                    //_logApp.CreateLog(new LogDTO(ModuleConstants.Address, ActionConstants.View, TokenHelper.GetUserId(httpContext)));
                    return new ResponseDTO<AddressDTO>(statusCode: StatusCodes.Status404NotFound, message: OperationMessage.STATUS_404_NOT_FOUND);
                }

                return new ResponseDTO<AddressDTO>(statusCode: StatusCodes.Status200OK, entity: new AddressDTO(address));
            }
            catch (Exception ex)
            {
                return new ResponseDTO<AddressDTO>(statusCode: StatusCodes.Status500InternalServerError, message: ex.Message);
            }
        }

        public ResponseDTO<object> Delete(HttpContext httpContext, Guid id)
        {
            try
            {
                var address = GetAddressById(id);
                if (address == null)
                    return new ResponseDTO<object>(statusCode: StatusCodes.Status404NotFound, message: OperationMessage.STATUS_404_NOT_FOUND);

                _unitOfWork.AddressRepository.Delete(address);
                if (_unitOfWork.Commit() > 0)
                {
                    //_logApp.CreateLog(new LogDTO(ModuleConstants.Address, ActionConstants.Delete, TokenHelper.GetUserId(httpContext)));
                    return new ResponseDTO<object>(statusCode: StatusCodes.Status200OK);
                }

                return new ResponseDTO<object>(statusCode: StatusCodes.Status400BadRequest, message: OperationMessage.STATUS_400_BAD_REQUEST);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<object>(statusCode: StatusCodes.Status500InternalServerError, message: ex.Message);
            }
        }
        #endregion

        #region [GetUserAddress]
        public ResponseDTO<AddressDTO> GetProfileAddress(HttpContext httpContext)
        {
            try
            {
                var address = _unitOfWork.AddressRepository.Get(x => x.UserId.Equals(TokenHelper.GetUserId(httpContext)));

                if (address == null)
                    return new ResponseDTO<AddressDTO>(statusCode: StatusCodes.Status404NotFound, message: OperationMessage.STATUS_404_NOT_FOUND);

                return new ResponseDTO<AddressDTO>(statusCode: StatusCodes.Status200OK, entity: new AddressDTO(address));
            }
            catch(Exception ex)
            {
                return new ResponseDTO<AddressDTO>(statusCode: StatusCodes.Status500InternalServerError, message: ex.Message);
            }
        }
        #endregion

        #region [Helpers]
        #region [Validation]
        private List<string> Validate(AddressDTO addressDTO)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(addressDTO.Street))
            {
                errors.Add("Rua não pode ser nula");
            }

            return errors;
        }
        #endregion

        private Address? GetAddressById(Guid id) => 
            _unitOfWork.AddressRepository.Get(x => x.Id == id);
        #endregion
    }
}
