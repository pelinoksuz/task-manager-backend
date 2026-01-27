namespace task_manager_backend.Domain.Entities;

public class ActivityLog
{
    public int Id { get; set; }
    public string User { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public DateTime Time { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
