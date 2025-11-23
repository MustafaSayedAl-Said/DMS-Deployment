using DMS.Core.Dto;

namespace DMS.Services.Interfaces
{
    public interface IWorkspaceService
    {
        Task<List<WorkspaceDto>> GetAllWorkspacesAsync();

        Task<WorkspaceDto> GetWorkspaceByIdAsync(int id);

        Task<WorkspaceDto> GetWorkspaceByUserIdAsync(int userId);

        Task<bool> AddWorkspaceAsync(WorkspaceDto workspaceDto);

        Task<bool> UpdateWorkspaceAsync(WorkspaceDto workspaceDto);

        Task<bool> DeleteWorkspaceAsync(int id);
    }
}
