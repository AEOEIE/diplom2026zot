namespace NewDiplom.Client.DTOs.Audit;

public class AuditLogDto
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string? UserLogin { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string? RecordId { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AuditLogResponseDto
{
    public List<AuditLogDto> Items { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}