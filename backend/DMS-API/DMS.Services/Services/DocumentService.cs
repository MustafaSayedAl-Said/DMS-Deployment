using AutoMapper;
using DMS.Core.Dto;
using DMS.Core.Interfaces;
using DMS.Core.Sharing;
using DMS.Services.Interfaces;

namespace DMS.Services.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IUnitOfWork _uOW;
        private readonly IMapper _mapper;

        public DocumentService(IUnitOfWork UOW, IMapper mapper, IRabbitMQService rabbitMQService)
        {
            _uOW = UOW;
            _mapper = mapper;
        }

        public async Task<(IReadOnlyList<DocumentGetDto>, int)> GetAllDocumentsAsync(DocumentParams documentParams)
        {

            var (allDocuments, totalItems) = await _uOW.documentRepository.GetAllAsync(documentParams);
            var documents = _mapper.Map<IReadOnlyList<DocumentGetDto>>(allDocuments);

            if (documents is not null)
            {
                return (documents, totalItems);
            }
            throw new Exception("Error while retrieving documents");

        }

        public async Task<DocumentGetDto> GetDocumentByIdAsync(int id)
        {
            var document = await _uOW.documentRepository.GetAsync(id);
            return _mapper.Map<DocumentGetDto>(document);
        }

        public async Task<bool> AddDocumentAsync(DocumentDto documentDto, string name)
        {
            return await _uOW.documentRepository.AddSync(documentDto, name);
        }

        public async Task<bool> UpdateDocumentAsync(DocumentDto documentDto)
        {
            if (documentDto == null)
                throw new ArgumentNullException(nameof(documentDto));
            if (_uOW.documentRepository.documentExists(documentDto.Id))
            {
                return await _uOW.documentRepository.UpdateAsync(documentDto);
            }
            throw new Exception($"Document Not Found, Id [{documentDto.Id}] is Incorrect");
        }

        public async Task<bool> SoftDeleteDocumentAsync(int id)
        {

            var res = await _uOW.documentRepository.SoftDeleteDocumentAsync(id);
            return true;
        }

        public async Task<bool> VerifyDirectoryOwnershipAsync(int directoryId, int userId)
        {
            if (!_uOW.directoryRepository.directoryExists(directoryId))
                throw new Exception("Directory doesn't exist");

            var directory = await _uOW.directoryRepository.GetDirectoryWithWorkspaceAsync(directoryId);

            return directory.Workspace.UserId == userId;

        }

        public async Task<bool> VerifyDocumentOwnershipAsync(int id, string email)
        {
            if (!_uOW.documentRepository.documentExists(id))
                throw new Exception("Document doesn't exist");

            var document = await _uOW.documentRepository.GetAsync(id);

            return document.OwnerName == email;
        }

        public async Task<bool> UpdateDocumentVisibilityAsync(int id)
        {
            var res = await _uOW.documentRepository.UpdateDocumentVisibilityAsync(id);

            return res;
        }

        public async Task<(List<DocumentGetDto>, int)> GetAllPublicDocumentsAsync(DocumentParams documentParams)
        {
            var (allDocuments, totalItems) = await _uOW.documentRepository.GetAllPublicAsync(documentParams);
            var documents = _mapper.Map<List<DocumentGetDto>>(allDocuments);
            if (documents is not null)
            {
                return (documents, totalItems);
            }
            throw new Exception("Error while retrieving documents");

        }
    }
}
