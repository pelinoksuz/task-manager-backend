namespace task_manager_backend.Application.DTOs;

public class ActivityLogDto
{
    public int Id { get; set; }
    public string User { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
}

public class CreateActivityLogDto
{
    public string User { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
}
