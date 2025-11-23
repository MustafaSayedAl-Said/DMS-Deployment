using AutoMapper;
using DMS.Core.Interfaces;
using DMS.Infrastructure.Data;

namespace DMS.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;


        public IWorkspaceRepository workspaceRepository { get; }

        public IDirectoryRepository directoryRepository { get; }

        public IUserRepository userRepository { get; }

        public IDocumentRepository documentRepository { get; }

        public IActionLogRepository actionLogRepository { get; }

        public UnitOfWork(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            workspaceRepository = new WorkspaceRepository(_context);
            directoryRepository = new DirectoryRepository(_context);
            userRepository = new UserRepository(_context);
            documentRepository = new DocumentRepository(_context, _mapper);
            actionLogRepository = new ActionLogRepository(_context);
        }
    }
}
