using DMS.Core.Dto;
using DMS.Core.Entities;
using DMS.Core.Interfaces;
using DMS.Core.Sharing;
using DMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Repositories
{
    public class DirectoryRepository : GenericRepository<MyDirectory>, IDirectoryRepository
    {
        private readonly DataContext _context;
        public DirectoryRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public bool directoryExists(int id)
        {
            return _context.Directories.Any(d => d.Id == id);
        }

        public async Task<(IEnumerable<MyDirectory>, int TotalCount)> GetAllAsync(DirectoryParams directoryParams)
        {
            List<MyDirectory> query;

            //search by WorkspaceId
            query = await _context.Directories.AsNoTracking().Where(d => d.WorkspaceId == directoryParams.WorkspaceId && d.IsDeleted == false).ToListAsync();
            //if (directoryParams.WorkspaceId.HasValue)
            //{

            //}
            //else
            //{
            //    query = await _context.Directories.AsNoTracking().ToListAsync();
            //}

            //search by name
            if (!string.IsNullOrEmpty(directoryParams.Search))
                query = query.Where(x => x.Name.ToLower().Contains(directoryParams.Search.ToLower())).ToList();

            int totalCount = query.Count;

            //sorting
            if (!string.IsNullOrEmpty(directoryParams.Sort))
            {
                query = directoryParams.Sort switch
                {
                    "NameAsc" => query.OrderBy(x => x.Name).ToList(),
                    "NameDesc" => query.OrderByDescending(x => x.Name).ToList(),
                    _ => query.ToList(),
                };
            }

            //paging
            query = query.Skip((directoryParams.PageSize) * (directoryParams.PageNumber - 1)).Take(directoryParams.PageSize).ToList();

            return (query, totalCount);
        }

        public async Task<MyDirectory> GetDirectoryWithDocumentsAsync(int id)
        {
            return await _context.Directories.Include(d => d.Documents).FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<MyDirectory> GetDirectoryWithWorkspaceAsync(int id)
        {
            return await _context.Directories.Include(d => d.Workspace).FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<bool> SoftDeleteDirectoryAsync(int id)
        {
            var directory = await _context.Directories.Include(d => d.Documents).FirstOrDefaultAsync(d => d.Id == id);

            if (directory == null)
            {
                throw new Exception("Something Went Wrong");
            }

            directory.IsDeleted = true;

            foreach (var document in directory.Documents)
            {
                document.IsDeleted = true;
            }

            _context.Directories.Update(directory);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateDirectoryNameAsync(string newName, int directoryId)
        {
            var directory = await _context.Directories.FirstOrDefaultAsync(d => d.Id == directoryId);

            if (directory == null)
            {
                throw new Exception("Something went wrong");
            }

            directory.Name = newName;
            _context.Directories.Update(directory);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<WorkspaceDto> GetWorkspaceByDirectoryIdAsync(int id)
        {
            var directory = await _context.Directories.Include(d => d.Workspace).FirstOrDefaultAsync(d => d.Id == id);

            if (directory == null)
            {
                throw new Exception("Something went wrong");
            }

            var workspaceDto = new WorkspaceDto
            {
                Name = directory.Workspace.Name,
                id = directory.Workspace.Id,
            };

            return workspaceDto;
        }

        //public async Task<ICollection<MyDirectory>> GetDirectoriesInWorkspace(int workspaceId)
        //{
        //    var directories = await _context.Directories.AsNoTracking().Where(d => d.WorkspaceId == workspaceId).ToListAsync();

        //    return directories;
        //}
    }
}
