using Microsoft.AspNetCore.Identity;

namespace DMS.Core.Entities
{
    public class User : IdentityUser<int>
    {
        public string DisplayName { get; set; }

        public bool isLocked { get; set; } = false;

        public virtual Workspace Workspace { get; set; }
    }
}
