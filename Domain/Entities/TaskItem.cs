namespace task_manager_backend.Domain.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public bool IsCompleted { get; set; }
    public bool IsDeleted { get; set; }
}
