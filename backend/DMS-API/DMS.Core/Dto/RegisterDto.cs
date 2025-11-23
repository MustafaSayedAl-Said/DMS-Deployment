using System.ComponentModel.DataAnnotations;

namespace DMS.Core.Dto
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "Min Length is 3 characters")]
        public string DisplayName { get; set; }

        public string Password { get; set; }

        [Required]
        public string WorkspaceName { get; set; }
    }
}
