using DMS.API.Helpers;
using DMS.Core.Dto;
using DMS.Core.Sharing;
using DMS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DMS.API.Controllers
{
    [Route("api/directories")]
    [ApiController]
    public class DirectoriesController : ControllerBase
    {
        private readonly IDirectoryService _directoryService;
        public DirectoriesController(IDirectoryService directoryService)
        {
            _directoryService = directoryService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Get([FromQuery] DirectoryParams directoryParams)
        {
            try
            {
                var userId = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User is not authenticated");
                }

                var (directories, totalItems) = await _directoryService.GetAllDirectoriesAsync(directoryParams);
                return Ok(new Pagination<MyDirectoryDto>(totalItems, directoryParams.PageSize, directoryParams.PageNumber, directories));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("{id}")]

        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var directoryDto = await _directoryService.GetDirectoryByIdAsync(id);
                return Ok(directoryDto);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Post(MyDirectoryDto directoryDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userId = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                    // Check if the user has the "Admin" role from the token
                    var isAdmin = HttpContext.User.IsInRole("Admin");
                    if (!isAdmin)
                    {
                        // Verify if the workspace belongs to the user
                        var isOwner = await _directoryService.VerifyWorkspaceOwnershipAsync(directoryDto.WorkspaceId, int.Parse(userId));

                        if (!isOwner)
                        {
                            return Forbid("User is not authorized to access this workspace");
                        }
                    }
                    var result = await _directoryService.AddDirectoryAsync(directoryDto);
                    if (result)
                        return Ok(directoryDto);

                    return BadRequest("Error occurred while adding the directory");
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var userId = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

                // Check if the user has the "Admin" role from the token
                var isAdmin = HttpContext.User.IsInRole("Admin");
                if (!isAdmin)
                {
                    var isOwner = await _directoryService.VerifyDirectoryOwnershipAsync(id, int.Parse(userId));

                    if (!isOwner)
                    {
                        return Forbid("User is not authorized to delete this directory");
                    }

                }

                var result = await _directoryService.SoftDeleteDirectoryAsync(id);

                if (result)
                {
                    return Ok("Directory and its documents were soft deleted successfully");
                }
                return BadRequest("Error occurred while deleting the directory");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<ActionResult> Patch(int id, [FromBody] string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                return BadRequest("Invalid patch document");
            }
            try
            {
                var userId = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                // Check if the user has the "Admin" role from the token
                var isAdmin = HttpContext.User.IsInRole("Admin");
                if (!isAdmin)
                {
                    var isOwner = await _directoryService.VerifyDirectoryOwnershipAsync(id, int.Parse(userId));

                    if (!isOwner)
                    {
                        return Forbid("User is not authorized to modify this directory");
                    }
                }

                var result = await _directoryService.UpdateDirectoryNameAsync(id, newName);

                if (!result)
                {
                    return BadRequest("Error occurred while updating the directory name");
                }
                return Ok("Directory name updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("user")]
        [Authorize]
        public async Task<IActionResult> GetWithoutWorkspaceId([FromQuery] DirectoryParams directoryParams)
        {
            try
            {
                var userId = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                var (directories, totalItems) = await _directoryService.GetAllDirectoriesByUserIdAsync(int.Parse(userId), directoryParams);
                return Ok(new Pagination<MyDirectoryDto>(totalItems, directoryParams.PageSize, directoryParams.PageNumber, directories));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("workspace/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetWorkspaceByDirectoryId(int id)
        {
            try
            {
                var workspaceDto = await _directoryService.GetWorkspaceByDirectoryId(id);

                return Ok(workspaceDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
