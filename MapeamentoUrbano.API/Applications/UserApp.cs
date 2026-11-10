using MapeamentoUrbano.API.Generics;
using MapeamentoUrbano.API.Helper;
using MapeamentoUrbano.Data.Context;
using MapeamentoUrbano.Data.UnitOfWork;
using MapeamentoUrbano.Domain.Constants;
using MapeamentoUrbano.Domain.DTOs.Base;
using MapeamentoUrbano.Domain.DTOs.LogDTO;
using MapeamentoUrbano.Domain.DTOs.User;
using MapeamentoUrbano.Domain.Entities;
using MapeamentoUrbano.Domain.Enums;
using MapeamentoUrbano.Domain.Helpers;
using MapeamentoUrbano.Domain.Interfaces.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace MapeamentoUrbano.API.Applications
{
    public class UserApp
    {
        private readonly string _defaultPassword;
        private readonly LogApp _logApp;
        private readonly PasswordHelper _passwordHelper;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public UserApp(LogApp logApp, PasswordHelper passwordHelper, CoreContext context, IConfiguration configuration)
        {
            _logApp = logApp;
            _passwordHelper = passwordHelper;
            _unitOfWork = new UnitOfWork(context);
            _configuration = configuration;
            _defaultPassword = _configuration.GetValue<string>("DEFAULT_PASSWORD");
        }

        #region [CRUD]
        public ResponseDTO<UserDTO> Create(HttpContext httpContext, UserDTO userDTO)
        {
            try
            {
                #region [Validation]
                List<string> validationErrors = Validate(userDTO);
                if (validationErrors.Count > 0)
                    return new ResponseDTO<UserDTO>(statusCode: StatusCodes.Status400BadRequest, validationErrors: validationErrors);
                #endregion

                User user = userDTO.MapperToEntity();

                user.UserNameNormalized = userDTO.UserName.StringNormalization();
                user.FirstAccess = true;

                User loggedUser = GetUserById(TokenHelper.GetUserId(httpContext));

                if(loggedUser != null && loggedUser.RoleId.Equals(VariableConstants.MasterId))
                {
                    user.RoleId = VariableConstants.AdminId;
                    user.UserPassword = _passwordHelper.HashPassword(_defaultPassword);
                }
                else
                {
                    user.UserPassword = _passwordHelper.HashPassword(userDTO.UserPassword);
                    user.RoleId = VariableConstants.UserId;
                }
 
                _unitOfWork.UserRepository.Add(user);

                if (_unitOfWork.Commit() > 0)
                { 
                    userDTO.Id = user.Id;
                    //_logApp.CreateLog(new LogDTO(ModuleConstants.User, ActionConstants.Create, TokenHelper.GetUserId(httpContext)));
                    return new ResponseDTO<UserDTO>(statusCode: StatusCodes.Status201Created, entity: userDTO);
                }

                return new ResponseDTO<UserDTO>(statusCode: StatusCodes.Status400BadRequest, message: OperationMessage.STATUS_400_INVALID_PAYLOAD_INCONSISTENT);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<UserDTO>(statusCode: StatusCodes.Status500InternalServerError, message: ex.Message);
            }
        }

        public ResponseDTO<UserDTO> Update(HttpContext httpContext, UserDTO userDTO)
        {
            try
            {
                #region [Validation]
                List<string> validationErrors = Validate(userDTO);
                if (validationErrors.Count > 0)
                    return new ResponseDTO<UserDTO>(statusCode: StatusCodes.Status400BadRequest, validationErrors: validationErrors);
                #endregion

                User? user = GetUserById(userDTO.Id);
                User? editingUser = GetUserById(TokenHelper.GetUserId(httpContext));

                if (user != null && editingUser != null)
                {
                    if ((!(user.RoleId.Equals(VariableConstants.MasterId))) && ((user.Id == editingUser.Id) || (editingUser.RoleId.Equals(VariableConstants.MasterId))))
                    {
                        user.MapperToEntity(userDTO);
                        user.UserNameNormalized = userDTO.UserName.StringNormalization();

                        _unitOfWork.UserRepository.AddOrUpdate(user);

                        if (_unitOfWork.Commit() > 0)
                        {
                            //_logApp.CreateLog(new LogDTO(ModuleConstants.User, ActionConstants.Update, TokenHelper.GetUserId(httpContext)));
                            return new ResponseDTO<UserDTO>(statusCode: StatusCodes.Status200OK, entity: userDTO);
                        }

                        return new ResponseDTO<UserDTO>(statusCode: StatusCodes.Status400BadRequest, message: OperationMessage.STATUS_400_INVALID_PAYLOAD_INCONSISTENT);
                    }

                    return new ResponseDTO<UserDTO>(statusCode: StatusCodes.Status400BadRequest, message: OperationMessage.STATUS_400_BAD_REQUEST);
                }

                return new ResponseDTO<UserDTO>(statusCode: StatusCodes.Status400BadRequest, message: OperationMessage.STATUS_400_INVALID_PAYLOAD_INCONSISTENT);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<UserDTO>(statusCode: StatusCodes.Status500InternalServerError, message: ex.Message);
            }
        }

        public ResponseDTO<UserDTO> Get(HttpContext httpContext, Guid id)
        {
            try
            {
                var user = GetUserById(id);
                if (user == null)
                {
                    //_logApp.CreateLog(new LogDTO(ModuleConstants.User, ActionConstants.View, TokenHelper.GetUserId(httpContext)));
                    return new ResponseDTO<UserDTO>(statusCode: StatusCodes.Status404NotFound, message: OperationMessage.STATUS_404_NOT_FOUND);
                }

                return new ResponseDTO<UserDTO>(statusCode: StatusCodes.Status200OK, entity: new UserDTO(user));
            }
            catch (Exception ex)
            {
                return new ResponseDTO<UserDTO>(statusCode: StatusCodes.Status500InternalServerError, message: ex.Message);
            }
        }

        public ResponseDTO<object> Delete(HttpContext httpContext, Guid id)
        {
            try
            {
                var user = GetUserById(id);
                if (user == null)
                    return new ResponseDTO<object>(statusCode: StatusCodes.Status404NotFound, message: OperationMessage.STATUS_404_NOT_FOUND);

                if (user.RoleId.Equals(VariableConstants.MasterId))
                    return new ResponseDTO<object>(statusCode: StatusCodes.Status400BadRequest, message: OperationMessage.STATUS_400_BAD_REQUEST);

                _unitOfWork.UserRepository.Delete(user);
                if (_unitOfWork.Commit() > 0)
                {
                    //_logApp.CreateLog(new LogDTO(ModuleConstants.User, ActionConstants.Delete, TokenHelper.GetUserId(httpContext)));
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

        #region [Paginated]
        public async Task<PaginatedItemsDTO<PaginatedUserDTO>> GetListAsync(string? name, string? sortOrder, int pageSize, int pageIndex)
        {
            IQueryable<User> users = _unitOfWork.UserRepository.GetAll().Include(x => x.Role);

            #region [Filter]
            if (!string.IsNullOrEmpty(name))
                users = users.Where(x => x.UserNameNormalized.Contains(name.StringNormalization()));
            #endregion

            #region [Order]
            Enum.TryParse(value: sortOrder, ignoreCase: true, out SortOrder filterSortOrder);

            var itemsOrderedEnumerable = filterSortOrder switch
            {
                SortOrder.NameDesc => GenericSort.SortByDescending(users, a => a.UserName),
                SortOrder.Name => GenericSort.SortByAscending(users, a => a.UserName),
                _ => GenericSort.SortByAscending(users, a => a.UserName)
            };
            #endregion

            var itemsOnPage = await GenericSort.SkipTakeAndSelectItemsAsync(itemsOrderedEnumerable, pageSize, pageIndex, user => new PaginatedUserDTO(user));
            var totalItems = await users.CountAsync();

            return new PaginatedItemsDTO<PaginatedUserDTO>(pageIndex, pageSize, totalItems, itemsOnPage);
        }
        #endregion

        #region [GetUserProfile]
        public ResponseDTO<UserProfileDTO> GetUserProfile(HttpContext httpContext)
        {
            try
            {
                User user = GetUserById(TokenHelper.GetUserId(httpContext));

                if (user == null)
                    return new ResponseDTO<UserProfileDTO>(statusCode: StatusCodes.Status404NotFound, message: OperationMessage.STATUS_404_NOT_FOUND);

                return new ResponseDTO<UserProfileDTO>(statusCode: StatusCodes.Status200OK, entity: new UserProfileDTO(user));
            }
            catch(Exception ex)
            {
                return new ResponseDTO<UserProfileDTO>(statusCode: StatusCodes.Status500InternalServerError, message: ex.Message);
            }
        }
        #endregion

        #region [Password]
        internal ResponseDTO<object> ResetPassword(HttpContext httpContext, Guid id)
        {
            try
            {
                var user = GetUserById(id);

                if (user == null)
                    return new ResponseDTO<object>(statusCode: StatusCodes.Status404NotFound, message: OperationMessage.STATUS_404_NOT_FOUND);

                if (user.RoleId.Equals(VariableConstants.MasterId))
                    return new ResponseDTO<object>(statusCode: StatusCodes.Status400BadRequest, message: OperationMessage.STATUS_400_BAD_REQUEST);

                user.UserPassword = _passwordHelper.HashPassword(_defaultPassword);
                user.FirstAccess = true;

                _unitOfWork.UserRepository.Update(user);
                if (_unitOfWork.Commit() > 0)
                {
                    //_logApp.CreateLog(new LogDTO(ModuleConstants.User, ActionConstants.ResetPassword, TokenHelper.GetUserId(httpContext)));
                    return new ResponseDTO<object>(statusCode: StatusCodes.Status200OK);
                }

                return new ResponseDTO<object>(statusCode: StatusCodes.Status400BadRequest, message: OperationMessage.STATUS_400_INVALID_PAYLOAD_INCONSISTENT);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<object>(statusCode: StatusCodes.Status500InternalServerError, message: ex.Message);
            }
        }

        internal ResponseDTO<object> ChangePassword(HttpContext httpContext, string password)
        {
            try
            {
                var user = GetUserById(TokenHelper.GetUserId(httpContext));

                if (user == null)
                    return new ResponseDTO<object>(statusCode: StatusCodes.Status404NotFound, message: OperationMessage.STATUS_404_NOT_FOUND);

                if (user.RoleId.Equals(VariableConstants.MasterId))
                    return new ResponseDTO<object>(statusCode: StatusCodes.Status400BadRequest, message: OperationMessage.STATUS_400_BAD_REQUEST);

                user.UserPassword = _passwordHelper.HashPassword(password);

                _unitOfWork.UserRepository.Update(user);
                if (_unitOfWork.Commit() > 0)
                {
                    //_logApp.CreateLog(new LogDTO(ModuleConstants.User, ActionConstants.ChangePassword, user.Id));
                    return new ResponseDTO<object>(statusCode: StatusCodes.Status200OK);
                }

                return new ResponseDTO<object>(statusCode: StatusCodes.Status400BadRequest, message: OperationMessage.STATUS_400_INVALID_PAYLOAD_INCONSISTENT);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<object>(statusCode: StatusCodes.Status500InternalServerError, message: ex.Message);
            }
        }
        #endregion

        #region [Helpers]
        #region [Validation]
        private List<string> Validate(UserDTO userDTO)
        {
            var errors = new List<string>();

            if (!string.IsNullOrEmpty(userDTO.UserName))
            {
                errors.AddRange(StringHelper.ValidateString(userDTO.UserName, VariableConstants.UserName));

                if (DoesUserNameExists(userDTO))
                    errors.Add("Já existe um usuário cadastrado com esse nome de usuário.");
            }
            else
            {
                errors.Add("Nome não pode ser nulo");
            }
            if (DoesUserEmailExists(userDTO))
                errors.Add("Já existe um usuário cadastrado com esse e-mail.");

            return errors;
        }
        #endregion

        private bool DoesUserNameExists(UserDTO user) =>
            _unitOfWork.UserRepository.GetAll(x => x.UserName == user.UserName && x.Id != user.Id).Any();

        private bool DoesUserEmailExists(UserDTO user) =>
            _unitOfWork.UserRepository.GetAll(x => x.UserEmail == user.UserEmail && x.Id != user.Id).Any();

        internal User? GetUserById(Guid id) =>
            _unitOfWork.UserRepository.GetAll(x => x.Id.Equals(id)).Include(x => x.Role).FirstOrDefault();

        public User? GetUserByLogin(string email) =>
            _unitOfWork.UserRepository.GetAll(x => x.UserEmail.Equals(email)).Include(x => x.Role).FirstOrDefault();
        #endregion
    }
}