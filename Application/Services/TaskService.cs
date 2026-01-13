using task_manager_backend.Domain.Entities;
using task_manager_backend.Application.DTOs;
using task_manager_backend.Application.Interfaces;

namespace task_manager_backend.Application.Services;

public class TaskService : ITaskService
{
    private static readonly List<TaskItem> _tasks = new()
    {
        new() { Id = 1, Title = "Learn backend basics", IsCompleted = false },
        new() { Id = 2, Title = "Connect frontend to API", IsCompleted = false }
    };

    public List<TaskItem> GetAll()
    {
        return _tasks.Where(t => !t.IsDeleted).ToList();
    }

    public TaskItem Create(CreateTaskDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new ArgumentException("Title is required");

        var task = new TaskItem
        {
            Id = _tasks.Max(t => t.Id) + 1,
            Title = dto.Title,
            IsCompleted = false,
            IsDeleted = false
        };

        _tasks.Add(task);
        return task;
    }

    public void MarkAsCompleted(int id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id && !t.IsDeleted);

        if (task == null)
            throw new KeyNotFoundException("Task not found");

        if (task.IsCompleted)
            throw new InvalidOperationException("Task is already completed");

        task.IsCompleted = true;
    }

    public void SoftDelete(int id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);

        if (task == null)
            throw new KeyNotFoundException("Task not found");

        task.IsDeleted = true;
    }
}
