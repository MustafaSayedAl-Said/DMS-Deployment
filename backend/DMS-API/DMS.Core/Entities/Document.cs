using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.Core.Entities
{
    public class Document : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [ForeignKey("MyDirectory")]
        public int DirectoryId { get; set; }

        public string DocumentContent { get; set; }

        public string OwnerName { get; set; }

        public DateTime ModifyDate { get; set; }

        public bool IsPublic { get; set; } = false;

        public bool IsDeleted { get; set; } = false;

        public virtual MyDirectory MyDirectory { get; set; }
    }
}
