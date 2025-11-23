using AutoMapper;
using DMS.Core.Dto;
using DMS.Core.Entities;
using DMS.Core.Sharing;
using DMS.Services.Extensions;
using DMS.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DMS.Services.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public UserService(UserManager<User> userManager, SignInManager<User> signInManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<UserDto> UserLogin(LoginDto loginDto)
        {
            if (loginDto == null)
                throw new ArgumentNullException(nameof(loginDto));

            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user is null)
                throw new Exception("Email Doesn't exist");

            if (user.isLocked)
            {
                throw new Exception("Your account is locked. Please contact website admin.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
                throw new Exception("Password is wrong");

            var isAdminCheck = await _userManager.IsInRoleAsync(user, "Admin");

            return new UserDto
            {
                DisplayName = user.DisplayName,
                Email = loginDto.Email,
                Token = await _tokenService.CreateToken(user),
                Id = user.Id,
                isAdmin = isAdminCheck,
            };
        }

        public async Task<UserDto> UserRegister(RegisterDto registerDto)
        {
            if (registerDto == null)
                throw new ArgumentNullException(nameof(registerDto));
            if (CheckEmailExistance(registerDto.Email).Result)
            {
                throw new Exception("Email already exists");
            }
            var user = new User
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Email,
                Workspace = new Workspace
                {
                    Name = registerDto.WorkspaceName,
                }
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded == false)
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                throw new Exception(string.Join("\n", errors));
            }

            var CreatedUser = await _userManager.FindByEmailAsync(registerDto.Email);

            if (CreatedUser is null)
                throw new Exception("Something Went Wrong");

            return new UserDto
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                Token = await _tokenService.CreateToken(CreatedUser),
                Id = CreatedUser.Id
            };
        }

        public async Task<UserDto> GetCurrentUser(HttpContext httpContext)
        {
            //var email = httpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;

            //if (email == null)
            //{
            //    throw new Exception("User is not authenticated");
            //}

            var user = await _userManager.FindEmailByClaim(httpContext.User);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            var isAdminCheck = await _userManager.IsInRoleAsync(user, "Admin");

            return new UserDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _tokenService.CreateToken(user),
                Id = user.Id,
                isAdmin = isAdminCheck,
            };
        }

        public async Task<bool> CheckEmailExistance(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        public async Task<WorkspaceDto> GetUserWorkspace(HttpContext httpContext)
        {
            //var email = httpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
            //if (email == null)
            //{
            //    throw new Exception("User is not authenticated");
            //}
            //var user = await _userManager.Users.Include(x => x.Workspace).SingleOrDefaultAsync(x => x.Email == email);
            var user = await _userManager.FindUserByClaimWithWorkspace(httpContext.User);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            var _result = _mapper.Map<WorkspaceDto>(user.Workspace);

            return _result;

        }

        public async Task<(List<UserGetDto>, int)> GetAllUsersAsync(UserParams userParams, int currentUserId)
        {

            var users = await _userManager.Users.Include(u => u.Workspace).Where(u => u.Id != currentUserId).ToListAsync();

            if (users == null || users.Count == 0)
                throw new Exception("Users not found");

            if (!string.IsNullOrEmpty(userParams.Search))
                users = users.Where(x => x.Email!.ToLower().Contains(userParams.Search.ToLower())).ToList();

            int totalCount = users.Count();

            if (!string.IsNullOrEmpty(userParams.Sort))
            {
                users = userParams.Sort switch
                {
                    "NameAsc" => users.OrderBy(x => x.UserName).ToList(),
                    "NameDesc" => users.OrderByDescending(x => x.UserName).ToList(),
                    "EmailAsc" => users.OrderBy(x => x.Email).ToList(),
                    "EmailDesc" => users.OrderByDescending(x => x.Email).ToList(),
                    "WorkspaceAsc" => users.OrderBy(x => x.Workspace.Name).ToList(),
                    "WorkspaceDesc" => users.OrderByDescending(x => x.Workspace.Name).ToList(),
                    _ => users.ToList()
                };
            }
            users = users.Skip((userParams.PageSize) * (userParams.PageNumber - 1)).Take(userParams.PageSize).ToList();
            var userDtos = _mapper.Map<List<UserGetDto>>(users);

            return (userDtos, totalCount);
        }

        public async Task<bool> ToggleUserLock(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                throw new Exception("User Id doesn't exist");

            user.isLocked = !user.isLocked;

            var res = await _userManager.UpdateAsync(user);

            if (res.Succeeded)
            {
                return true;
            }

            return false;

        }

        public async Task<List<int>> GetAllAdminIDs()
        {
            var users = await _userManager.GetUsersInRoleAsync("Admin");

            return users.Select(x => x.Id).ToList();
        }
    }
}
