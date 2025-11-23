using DMS.Core.Entities;

namespace DMS.Core.Interfaces
{
    public interface IWorkspaceRepository : IGenericRepository<Workspace>
    {
        public bool workspaceExists(int id);

        public int getUserId(int id);

        public Task<Workspace> getWorkspaceByUserId(int userId);
    }
}
