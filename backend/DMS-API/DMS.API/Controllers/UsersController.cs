using DMS.API.Errors;
using DMS.API.Helpers;
using DMS.Core.Dto;
using DMS.Core.Sharing;
using DMS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DMS.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]

        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userDto = await _userService.UserLogin(loginDto);
                    return Ok(userDto);
                }
                return BadRequest(new BaseCommonResponse(500));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userDto = await _userService.UserRegister(registerDto);
                    return Ok(userDto);
                }
                return BadRequest("Something went Wrong");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("test")]

        public ActionResult<string> Test()
        {
            return "hi";
        }


        [Authorize]
        [HttpGet("current")]

        public async Task<IActionResult> Get()
        {
            try
            {
                var userDto = await _userService.GetCurrentUser(HttpContext);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("check")]

        public async Task<IActionResult> CheckEmailExist([FromQuery] string email)
        {
            var res = await _userService.CheckEmailExistance(email);
            return Ok(res);
        }

        [Authorize]
        [HttpGet("workspace")]
        public async Task<IActionResult> GetUserWorkspace()
        {
            try
            {
                var res = await _userService.GetUserWorkspace(HttpContext);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserParams userParams)
        {
            try
            {
                var userId = HttpContext.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User is not authenticated");
                }
                int currentUserId = int.Parse(userId);
                var (users, totalItems) = await _userService.GetAllUsersAsync(userParams, currentUserId);
                return Ok(new Pagination<UserGetDto>(totalItems, userParams.PageSize, userParams.PageNumber, users));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{userId}")]
        public async Task<IActionResult> ToggleUserLock(int userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId.ToString()))
                {
                    return BadRequest("Must input a user Id");
                }

                var result = await _userService.ToggleUserLock(userId);
                if (!result)
                {
                    return BadRequest("Error occurred while locking user account");
                }
                return Ok("User lock was toggled successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}