using DMS.Core.Dto;
using DMS.Core.Entities;
using DMS.Core.Sharing;

namespace DMS.Core.Interfaces
{
    public interface IDirectoryRepository : IGenericRepository<MyDirectory>
    {
        public bool directoryExists(int id);

        public Task<(IEnumerable<MyDirectory>, int TotalCount)> GetAllAsync(DirectoryParams directoryParams);

        public Task<MyDirectory> GetDirectoryWithWorkspaceAsync(int id);

        public Task<MyDirectory> GetDirectoryWithDocumentsAsync(int id);

        public Task<bool> SoftDeleteDirectoryAsync(int id);

        public Task<bool> UpdateDirectoryNameAsync(string newName, int directoryId);

        public Task<WorkspaceDto> GetWorkspaceByDirectoryIdAsync(int id);
        //public Task<ICollection<MyDirectory>> GetDirectoriesInWorkspace(int workspaceId);
    }
}
