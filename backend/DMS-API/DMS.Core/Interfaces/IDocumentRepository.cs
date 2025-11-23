using DMS.Core.Dto;
using DMS.Core.Entities;
using DMS.Core.Sharing;

namespace DMS.Core.Interfaces
{
    public interface IDocumentRepository : IGenericRepository<Document>
    {
        public Task<(IEnumerable<Document>, int TotalCount)> GetAllAsync(DocumentParams documentParams);

        public Task<(IEnumerable<Document>, int)> GetAllPublicAsync(DocumentParams documentParams);

        public bool documentExists(int id);

        //public Task<ICollection<Document>> GetDocumentsInDirectory(int directoryId);

        Task<bool> AddSync(DocumentDto dto, string name);

        Task<bool> UpdateAsync(DocumentDto dto);

        public Task<bool> SoftDeleteDocumentAsync(int id);

        public Task<bool> UpdateDocumentVisibilityAsync(int id);
    }
}