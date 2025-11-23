namespace DMS.Core.Interfaces
{
    public interface IUnitOfWork
    {
        public IWorkspaceRepository workspaceRepository { get; }

        public IDirectoryRepository directoryRepository { get; }

        public IUserRepository userRepository { get; }

        public IDocumentRepository documentRepository { get; }

        public IActionLogRepository actionLogRepository { get; }
    }
}
