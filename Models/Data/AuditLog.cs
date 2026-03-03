namespace SPSUL.Models.Data
{
    public class AuditLog
    {
        public long AuditLogId { get; set; }
        public int? TeacherId { get; set; }
        public required string TeacherName { get; set; }
        public required string Action { get; set; }
        public required string Entity { get; set; }
        public string? EntityId { get; set; }
        public string? Detail { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
