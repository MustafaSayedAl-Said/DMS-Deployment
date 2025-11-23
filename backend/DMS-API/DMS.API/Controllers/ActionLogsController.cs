using DMS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMS.API.Controllers
{
    [Route("api/actionlogs")]
    [ApiController]
    public class ActionLogsController : ControllerBase
    {
        private readonly IActionLogService _actionLogService;

        public ActionLogsController(IActionLogService actionLogService)
        {
            _actionLogService = actionLogService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]

        public async Task<IActionResult> Get()
        {
            try
            {
                var actionLogs = await _actionLogService.GetAllActionLogs();
                return Ok(actionLogs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
