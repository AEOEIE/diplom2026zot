namespace NewDiplom.Entities
{
    public class User
    {
        public int Id { get; set; }

        public int RoleId { get; set; }

        public int? PostOfficeId { get; set; }

        public string LastName { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string? MiddleName { get; set; }

        public string Phone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Login { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string? PassportSeries { get; set; }

        public string? PassportNumber { get; set; }

        public string? PassportIssuedBy { get; set; }

        public DateTime? PassportIssueDate { get; set; }

        public string? RegistrationAddress { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public Role? Role { get; set; }

        public PostOffice? PostOffice { get; set; }
        public int AccessFailedCount { get; set; } = 0;
        public bool LockoutEnabled { get; set; } = true;
        public DateTime? LockoutEnd { get; set; }
    }
}
