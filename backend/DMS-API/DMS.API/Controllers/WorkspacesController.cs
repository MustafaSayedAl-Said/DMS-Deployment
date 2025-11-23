using DMS.Core.Dto;
using DMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DMS.API.Controllers
{
    [Route("api/workspaces")]
    [ApiController]
    public class WorkspacesController : ControllerBase
    {
        private readonly IWorkspaceService _workspaceService;
        public WorkspacesController(IWorkspaceService workspaceService)
        {
            _workspaceService = workspaceService;
        }

        [HttpGet]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(BaseCommonResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var workspaces = await _workspaceService.GetAllWorkspacesAsync();
                return Ok(workspaces);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(BaseCommonResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var workspaceDto = await _workspaceService.GetWorkspaceByIdAsync(id);
                return Ok(workspaceDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("user/{userId}")]

        public async Task<IActionResult> GetByUser(int userId)
        {
            try
            {
                var workspaceDto = await _workspaceService.GetWorkspaceByUserIdAsync(userId);
                return Ok(workspaceDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post(WorkspaceDto workspaceDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _workspaceService.AddWorkspaceAsync(workspaceDto);
                    return Ok(workspaceDto);
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [HttpPut]

        public async Task<ActionResult> Put(WorkspaceDto workspaceDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _workspaceService.UpdateWorkspaceAsync(workspaceDto);
                    return Ok(workspaceDto);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete("{id}")]

        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _workspaceService.DeleteWorkspaceAsync(id);
                    return Ok("Workspace was deleted!");
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
