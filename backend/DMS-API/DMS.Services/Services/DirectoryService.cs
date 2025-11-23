using AutoMapper;
using DMS.Core.Dto;
using DMS.Core.Entities;
using DMS.Core.Interfaces;
using DMS.Core.Sharing;
using DMS.Services.Interfaces;

namespace DMS.Services.Services
{
    public class DirectoryService : IDirectoryService
    {
        private readonly IUnitOfWork _uOW;
        private readonly IMapper _mapper;

        public DirectoryService(IUnitOfWork UOW, IMapper mapper)
        {
            _uOW = UOW;
            _mapper = mapper;
        }
        public async Task<bool> AddDirectoryAsync(MyDirectoryDto directoryDto)
        {
            if (_uOW.workspaceRepository.workspaceExists(directoryDto.WorkspaceId))
            {
                var directoryMap = _mapper.Map<MyDirectory>(directoryDto);
                await _uOW.directoryRepository.AddAsync(directoryMap);
                return true;
            }
            throw new Exception("Workspace Was Not Found");
        }

        public async Task<bool> SoftDeleteDirectoryAsync(int id)
        {
            var res = await _uOW.directoryRepository.SoftDeleteDirectoryAsync(id);

            return res;

        }

        public async Task<(List<MyDirectoryDto>, int)> GetAllDirectoriesAsync(DirectoryParams directoryParams)
        {
            if (_uOW.workspaceRepository.workspaceExists(directoryParams.WorkspaceId))
            {
                var (allDirectories, totalItems) = await _uOW.directoryRepository.GetAllAsync(directoryParams);
                var directories = _mapper.Map<List<MyDirectoryDto>>(allDirectories);
                return (directories, totalItems);
            }
            throw new Exception("Workspace doesn't exist");
        }

        public async Task<MyDirectoryDto> GetDirectoryByIdAsync(int id)
        {
            var directory = await _uOW.directoryRepository.GetAsync(id);
            if (directory == null)
                throw new Exception($"This id [{id}] was Not Found");

            return _mapper.Map<MyDirectoryDto>(directory);
        }

        public async Task<bool> UpdateDirectoryAsync(MyDirectoryDto directoryDto)
        {
            if (directoryDto == null)
                throw new ArgumentNullException(nameof(directoryDto));
            if (_uOW.directoryRepository.directoryExists(directoryDto.id))
            {
                var directoryMap = _mapper.Map<MyDirectory>(directoryDto);
                await _uOW.directoryRepository.UpdateAsync(directoryMap);
                return true;
            }
            throw new Exception($"Directory Not Found, ID [{directoryDto.id}] is Incorrect");
        }

        public async Task<bool> VerifyWorkspaceOwnershipAsync(int workspaceId, int userId)
        {
            if (!_uOW.workspaceRepository.workspaceExists(workspaceId))
                throw new Exception($"Workspace with id [{workspaceId}] doesn't exist");

            var workspace = await _uOW.workspaceRepository.GetAsync(workspaceId);

            if (workspace.UserId != userId)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> VerifyDirectoryOwnershipAsync(int directoryId, int userId)
        {
            if (!_uOW.directoryRepository.directoryExists(directoryId))
                throw new Exception("Directory doesn't exist");

            var directory = await _uOW.directoryRepository.GetDirectoryWithWorkspaceAsync(directoryId);

            return directory.Workspace.UserId == userId;

        }

        public async Task<bool> UpdateDirectoryNameAsync(int id, string newName)
        {

            var res = await _uOW.directoryRepository.UpdateDirectoryNameAsync(newName, id);

            return res;
        }

        public async Task<(List<MyDirectoryDto>, int)> GetAllDirectoriesByUserIdAsync(int userId, DirectoryParams directoryParams)
        {
            var workspace = await _uOW.workspaceRepository.getWorkspaceByUserId(userId);

            directoryParams.WorkspaceId = workspace.Id;

            return await GetAllDirectoriesAsync(directoryParams);

        }

        public async Task<WorkspaceDto> GetWorkspaceByDirectoryId(int id)
        {
            if (!_uOW.directoryRepository.directoryExists(id))
            {
                throw new Exception("Directory doesn't exist");
            }

            return await _uOW.directoryRepository.GetWorkspaceByDirectoryIdAsync(id);
        }
    }
}
