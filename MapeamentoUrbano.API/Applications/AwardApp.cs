using MapeamentoUrbano.API.Generics;
using MapeamentoUrbano.Data.Context;
using MapeamentoUrbano.Data.UnitOfWork;
using MapeamentoUrbano.Domain.Constants;
using MapeamentoUrbano.Domain.DTOs.Award;
using MapeamentoUrbano.Domain.DTOs.Base;
using MapeamentoUrbano.Domain.DTOs.User;
using MapeamentoUrbano.Domain.Entities;
using MapeamentoUrbano.Domain.Enums;
using MapeamentoUrbano.Domain.Helpers;
using MapeamentoUrbano.Domain.Interfaces.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace MapeamentoUrbano.API.Applications
{
    public class AwardApp
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserApp _userApp;

        public AwardApp(CoreContext context, UserApp userApp)
        {
            _unitOfWork = new UnitOfWork(context);
            _userApp = userApp;
        }

        public ResponseDTO<object> CreateAwards(HttpContext httpContext)
        {
            try
            {
                if (_userApp.GetUserById(TokenHelper.GetUserId(httpContext)).RoleId != VariableConstants.MasterId)
                    return new ResponseDTO<object>(statusCode: StatusCodes.Status403Forbidden, message: OperationMessage.STATUS_403_FORBIDDEN);

                DateTime now = DateTime.UtcNow;

                DateTime? lastAwardDate = _unitOfWork.AwardRepository.GetAll()
                    .OrderByDescending(x => x.AwardDate)
                    .Select(x => (DateTime?)x.AwardDate)
                    .FirstOrDefault();

                var ranking = _unitOfWork.EventRepository.GetAll()
                    .Where(x =>
                        (!lastAwardDate.HasValue || x.RegistrationDate > lastAwardDate.Value) &&
                        x.RegistrationDate <= now)
                    .GroupBy(x => x.UserId)
                    .Select(g => new
                    {
                        UserId = g.Key,
                        Points = g.Sum(x => x.Points)
                    })
                    .Where(x => x.Points >= 1)
                    .OrderByDescending(x => x.Points)
                    .ThenBy(x => x.UserId)
                    .ToList();

                if (!ranking.Any())
                    return new ResponseDTO<object>(statusCode: StatusCodes.Status400BadRequest, message: OperationMessage.STATUS_400_BAD_REQUEST);

                var medals = _unitOfWork.MedalsRepository.GetAll().ToList();

                var gold = medals.FirstOrDefault(x => x.Id == VariableConstants.GoldMedal);
                var silver = medals.FirstOrDefault(x => x.Id == VariableConstants.SilverMedal);
                var bronze = medals.FirstOrDefault(x => x.Id == VariableConstants.BronzeMedal);

                if (gold == null || silver == null || bronze == null)
                    return new ResponseDTO<object>(statusCode: StatusCodes.Status400BadRequest, message: "Uma ou mais medalhas não foram encontradas.");

                int position = 0;
                int? previousPoints = null;
                int index = 0;

                var awards = new List<Award>();

                foreach (var user in ranking)
                {
                    index++;

                    if (previousPoints != user.Points)
                        position = index;

                    previousPoints = user.Points;

                    Guid? medalId = position switch
                    {
                        1 => gold.Id,
                        2 => silver.Id,
                        3 => bronze.Id,
                        _ => null
                    };

                    if (!medalId.HasValue)
                        continue;

                    var award = new Award
                    {
                        Id = Guid.NewGuid(),
                        AwardDate = now,
                        Points = user.Points,
                        FirstView = true,
                        UserId = user.UserId,
                        MedalId = medalId.Value
                    };

                    _unitOfWork.AwardRepository.Add(award);
                    awards.Add(award);
                }

                if (_unitOfWork.Commit() > 0)
                    return new ResponseDTO<object>(statusCode: StatusCodes.Status201Created, message: OperationMessage.STATUS_201_CREATED, entity: new AwardDTO(now, ranking.Count, awards));

                return new ResponseDTO<object>(statusCode: StatusCodes.Status400BadRequest, message: OperationMessage.STATUS_400_BAD_REQUEST);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<object>(statusCode: StatusCodes.Status500InternalServerError, message: ex.Message);
            }
        }

        public async Task<PaginatedItemsDTO<PaginatedAwardDTO>> GetListAsync(string? sortOrder, int pageSize, int pageIndex)
        {
            var awards = await _unitOfWork.AwardRepository.GetAll().Include(x => x.User).AsNoTracking().ToListAsync();

            var groupedAwards = awards.GroupBy(x => x.AwardDate)
                .Select(g => new PaginatedAwardDTO
                {
                    AwardDate = g.Key,
                    UserGold = g.FirstOrDefault(x => x.MedalId == VariableConstants.GoldMedal)?.User.UserName,
                    UserGoldPoints = g.FirstOrDefault(x => x.MedalId == VariableConstants.GoldMedal)?.Points ?? 0,
                    UserSilver = g.FirstOrDefault(x => x.MedalId == VariableConstants.SilverMedal)?.User.UserName,
                    UserSilverPoints = g.FirstOrDefault(x => x.MedalId == VariableConstants.SilverMedal)?.Points ?? 0,
                    UserBronze = g.FirstOrDefault(x => x.MedalId == VariableConstants.BronzeMedal)?.User.UserName,
                    UserBronzePoints = g.FirstOrDefault(x => x.MedalId == VariableConstants.BronzeMedal)?.Points ?? 0,
                });

            #region [Order]
            Enum.TryParse(value: sortOrder, ignoreCase: true, out SortOrder filterSortOrder);

            var itemsOrderedEnumerable = filterSortOrder switch
            {
                SortOrder.DateDesc => groupedAwards.OrderByDescending(x => x.AwardDate),
                SortOrder.Date => groupedAwards.OrderBy(x => x.AwardDate),
                _ => groupedAwards.OrderByDescending(x => x.AwardDate)
            };
            #endregion

            var totalItems = itemsOrderedEnumerable.Count();
            var itemsOnPage = itemsOrderedEnumerable.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();


            return new PaginatedItemsDTO<PaginatedAwardDTO>(pageIndex, pageSize, totalItems, itemsOnPage);
        }

        public PaginatedItemsDTO<ProfilePaginatedAwardDTO> GetProfileAwardsPaginated(HttpContext httpContext, string? sortOrder, int pageSize, int pageIndex)
        {
            var userId = TokenHelper.GetUserId(httpContext);
            var awards = _unitOfWork.AwardRepository.GetAll()
                .Where(x => x.UserId == userId)
                .Include(x => x.Medal)
                .AsNoTracking()
                .ToList();

            #region [Order]
            Enum.TryParse(value: sortOrder, ignoreCase: true, out SortOrder filterSortOrder);
            var itemsOrderedEnumerable = filterSortOrder switch
            {
                SortOrder.DateDesc => awards.OrderByDescending(x => x.AwardDate),
                SortOrder.Date => awards.OrderBy(x => x.AwardDate),
                _ => awards.OrderByDescending(x => x.AwardDate)
            };
            #endregion

            var totalItems = itemsOrderedEnumerable.Count();
            var itemsOnPage = itemsOrderedEnumerable.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            var profileAwardsDTOs = itemsOnPage.Select(award => new ProfilePaginatedAwardDTO(award)).ToList();
            return new PaginatedItemsDTO<ProfilePaginatedAwardDTO>(pageIndex, pageSize, totalItems, profileAwardsDTOs);
        }

    }
}
