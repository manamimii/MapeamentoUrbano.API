using MapeamentoUrbano.API.Applications;
using MapeamentoUrbano.Domain.DTOs.Base;
using MapeamentoUrbano.Domain.DTOs.LogDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace MapeamentoUrbano.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LogController
    {
        public readonly LogApp _logApp;

        public LogController(LogApp logApp)
        {
            _logApp = logApp;
        }

        /// <summary>
        /// Returns a collection of Logs
        /// </summary>
        /// <returns>A collection of Logs</returns>
        [HttpGet("Paginated")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<PaginatedItemsDTO<PaginatedLogDTO>> GetListAsync(
            [FromQuery] string? user,
            [FromQuery] string? sortOrder,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 1)
        {
            return await _logApp.GetListAsync(user, sortOrder, pageSize, pageIndex);
        }
    }
}
