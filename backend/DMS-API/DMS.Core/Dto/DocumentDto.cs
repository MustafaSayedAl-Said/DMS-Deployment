using Microsoft.AspNetCore.Http;

namespace DMS.Core.Dto
{
    public class DocumentDto
    {
        public int Id { get; set; }

        public int DirectoryId { get; set; }

        public IFormFile DocumentContent { get; set; }
    }

    public class DocumentGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int DirectoryId { get; set; }

        public string DocumentContent { get; set; }

        public DateTime ModifyDate { get; set; }

        public bool IsPublic { get; set; }

        public bool IsDeleted { get; set; }

        public string OwnerName { get; set; }
    }
}
