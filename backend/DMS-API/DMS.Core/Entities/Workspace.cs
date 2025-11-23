using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.Core.Entities
{
    public class Workspace : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<MyDirectory> Directories { get; set; }
    }
}

