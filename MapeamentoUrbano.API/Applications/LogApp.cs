using MapeamentoUrbano.API.Generics;
using MapeamentoUrbano.API.Helper;
using MapeamentoUrbano.Data.Context;
using MapeamentoUrbano.Data.UnitOfWork;
using MapeamentoUrbano.Domain.DTOs.Base;
using MapeamentoUrbano.Domain.DTOs.LogDTO;
using MapeamentoUrbano.Domain.Entities;
using MapeamentoUrbano.Domain.Enums;
using MapeamentoUrbano.Domain.Interfaces.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace MapeamentoUrbano.API.Applications
{
    public class LogApp
    {
        private readonly IUnitOfWork _unitOfWork;

        public LogApp(CoreContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }

        #region [Create Log]
        public void CreateLog(LogDTO logDTO)
        { 
            LogBase log = new(logDTO);

            _unitOfWork.LogRepository.Add(log);

            _unitOfWork.Commit();
        }
        #endregion

        #region [Paginated] 
        public async Task<PaginatedItemsDTO<PaginatedLogDTO>> GetListAsync(string? user, string? sortOrder, int pageSize, int pageIndex)
        {
            IQueryable<LogBase> logs = _unitOfWork.LogRepository.GetAll().Include(x => x.User);

            #region [Filter] 
            if (!string.IsNullOrEmpty(user))
                logs = logs.Where(x => x.User.UserNameNormalized.Contains(user.StringNormalization()));
            #endregion 

            #region [Order]
            Enum.TryParse(value: sortOrder, ignoreCase: true, out SortOrder filterSortOrder);
             
            var itemsOrderedEnumerable = filterSortOrder switch
            { 
                SortOrder.DateDesc => GenericSort.SortByDescending(logs, a => a.CreationDate),
                SortOrder.Date => GenericSort.SortByAscending(logs, a => a.CreationDate),
                _ => GenericSort.SortByAscending(logs, a => a.CreationDate)
            };
            #endregion

            var itemsOnPage = await GenericSort.SkipTakeAndSelectItemsAsync(itemsOrderedEnumerable, pageSize, pageIndex, log => new PaginatedLogDTO(log));
            var totalItems = await logs.CountAsync();

            return new PaginatedItemsDTO<PaginatedLogDTO>(pageIndex, pageSize, totalItems, itemsOnPage);
        }
        #endregion
    }
}
