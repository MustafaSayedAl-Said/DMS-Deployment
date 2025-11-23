using DMS.Core.Entities;

namespace DMS.Services.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(User user);
    }
}
