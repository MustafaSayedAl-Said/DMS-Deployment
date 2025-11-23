using DMS.Core.Dto;
using DMS.Core.Sharing;

namespace DMS.Services.Interfaces
{
    public interface IDocumentService
    {
        public Task<(IReadOnlyList<DocumentGetDto>, int)> GetAllDocumentsAsync(DocumentParams documentParams);

        public Task<(List<DocumentGetDto>, int)> GetAllPublicDocumentsAsync(DocumentParams documentParams);

        public Task<DocumentGetDto> GetDocumentByIdAsync(int id);

        public Task<bool> AddDocumentAsync(DocumentDto documentDto, string name);

        public Task<bool> UpdateDocumentAsync(DocumentDto documentDto);

        public Task<bool> SoftDeleteDocumentAsync(int id);

        public Task<bool> VerifyDirectoryOwnershipAsync(int directoryId, int userId);

        public Task<bool> VerifyDocumentOwnershipAsync(int id, string email);

        public Task<bool> UpdateDocumentVisibilityAsync(int id);
    }
}
