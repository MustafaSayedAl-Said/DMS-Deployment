using AutoMapper;
using DMS.Core.Dto;
using DMS.Core.Entities;
using DMS.Core.Interfaces;
using DMS.Services.Interfaces;

namespace DMS.Services.Services
{
    public class WorkspaceService : IWorkspaceService
    {
        private readonly IUnitOfWork _uOW;
        private readonly IMapper _mapper;

        public WorkspaceService(IUnitOfWork UOW, IMapper mapper)
        {
            _uOW = UOW;
            _mapper = mapper;
        }
        public async Task<bool> AddWorkspaceAsync(WorkspaceDto workspaceDto)
        {
            if (workspaceDto == null)
            {
                throw new ArgumentNullException(nameof(workspaceDto));
            }

            var workspaceMap = _mapper.Map<Workspace>(workspaceDto);
            await _uOW.workspaceRepository.AddAsync(workspaceMap);
            return true;
        }

        public async Task<bool> DeleteWorkspaceAsync(int id)
        {
            if (!_uOW.workspaceRepository.workspaceExists(id))
            {
                throw new Exception($"Workspace with id [{id}] not found");
            }
            var userId = _uOW.workspaceRepository.getUserId(id);
            await _uOW.workspaceRepository.DeleteAsync(userId);
            await _uOW.userRepository.DeleteAsync(userId);
            return true;
        }

        public async Task<List<WorkspaceDto>> GetAllWorkspacesAsync()
        {
            var allWorkspaces = await _uOW.workspaceRepository.GetAllAsync();
            if (allWorkspaces == null)
            {
                throw new Exception("No workspaces found");
            }
            return _mapper.Map<List<WorkspaceDto>>(allWorkspaces);
        }

        public async Task<WorkspaceDto> GetWorkspaceByIdAsync(int id)
        {
            var workspace = await _uOW.workspaceRepository.GetAsync(id);
            if (workspace == null)
            {
                throw new Exception($"Unable to find workspace {id}");
            }
            return _mapper.Map<WorkspaceDto>(workspace);
        }

        public async Task<WorkspaceDto> GetWorkspaceByUserIdAsync(int userId)
        {
            //if (!_uOW.userRepository.userExists(userId))
            //{
            //    throw new Exception("User Not Found");
            //}
            var workspace = await _uOW.workspaceRepository.getWorkspaceByUserId(userId);
            if (workspace == null)
            {
                throw new Exception($"Workspace for user with id [{userId}] not found");
            }

            return _mapper.Map<WorkspaceDto>(workspace);
        }

        public async Task<bool> UpdateWorkspaceAsync(WorkspaceDto workspaceDto)
        {
            if (workspaceDto == null)
            {
                throw new ArgumentNullException(nameof(workspaceDto));
            }

            if (!_uOW.workspaceRepository.workspaceExists(workspaceDto.id))
            {
                throw new Exception($"Workspace with id [{workspaceDto.id}] not found");
            }

            var workspaceMap = _mapper.Map<Workspace>(workspaceDto);
            await _uOW.workspaceRepository.UpdateAsync(workspaceMap);
            return true;
        }
    }
}
