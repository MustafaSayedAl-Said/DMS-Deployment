using DMS.Core.Dto;
using DMS.Core.Sharing;
using Microsoft.AspNetCore.Http;

namespace DMS.Services.Interfaces
{
    public interface IUserService
    {
        public Task<UserDto> UserLogin(LoginDto loginDto);

        public Task<UserDto> UserRegister(RegisterDto registerDto);

        public Task<UserDto> GetCurrentUser(HttpContext httpContext);

        public Task<bool> CheckEmailExistance(string email);

        public Task<WorkspaceDto> GetUserWorkspace(HttpContext httpContext);

        public Task<(List<UserGetDto>, int)> GetAllUsersAsync(UserParams userParams, int currentUserId);

        public Task<bool> ToggleUserLock(int userId);

        public Task<List<int>> GetAllAdminIDs();
    }
}
