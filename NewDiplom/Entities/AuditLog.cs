namespace NewDiplom.Entities;

public class AuditLog
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string ActionType { get; set; } = string.Empty;

    public string TableName { get; set; } = string.Empty;

    public string? RecordId { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
}