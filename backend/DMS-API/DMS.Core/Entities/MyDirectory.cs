using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.Core.Entities
{
    public class MyDirectory : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [ForeignKey("Workspace")]
        public int WorkspaceId { get; set; }

        public bool IsDeleted { get; set; } = false;

        public virtual Workspace Workspace { get; set; }

        public virtual ICollection<Document> Documents { get; set; }
    }
}
