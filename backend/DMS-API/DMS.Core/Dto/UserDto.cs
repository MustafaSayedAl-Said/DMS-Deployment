namespace DMS.Core.Dto
{
    public class UserDto
    {
        public string Email { get; set; }

        public string DisplayName { get; set; }

        public string Token { get; set; }

        public int Id { get; set; }

        public bool isAdmin { get; set; } = false;

    }

    public class UserGetDto
    {
        public string Email { get; set; }

        public string DisplayName { get; set; }

        public int id { get; set; }

        public string WorkspaceName { get; set; }

        public int WorkspaceId { get; set; }

        public bool isLocked { get; set; }
    }
}
