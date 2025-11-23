using DMS.Core.Dto;
using DMS.Core.Sharing;

namespace DMS.Services.Interfaces
{
    public interface IDirectoryService
    {
        Task<(List<MyDirectoryDto>, int)> GetAllDirectoriesAsync(DirectoryParams directoryParams);

        Task<(List<MyDirectoryDto>, int)> GetAllDirectoriesByUserIdAsync(int userId, DirectoryParams directoryParams);

        Task<MyDirectoryDto> GetDirectoryByIdAsync(int id);

        Task<bool> AddDirectoryAsync(MyDirectoryDto directoryDto);

        Task<bool> UpdateDirectoryAsync(MyDirectoryDto directoryDto);

        Task<bool> SoftDeleteDirectoryAsync(int id);

        public Task<bool> VerifyWorkspaceOwnershipAsync(int workspaceId, int userId);

        public Task<bool> VerifyDirectoryOwnershipAsync(int directoryId, int userId);

        public Task<bool> UpdateDirectoryNameAsync(int id, string newName);

        public Task<WorkspaceDto> GetWorkspaceByDirectoryId(int id);

    }
}
