namespace NewDiplom.Entities
{
    public class PostOffice
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? WorkingHours { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
