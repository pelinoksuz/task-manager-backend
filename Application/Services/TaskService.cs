using Microsoft.EntityFrameworkCore;
using task_manager_backend.Domain.Entities;
using task_manager_backend.Application.DTOs;
using task_manager_backend.Application.Interfaces;
using task_manager_backend.Infrastructure.Data;

namespace task_manager_backend.Application.Services;

public class TaskService : ITaskService
{
    private readonly AutoOpsDbContext _context;

    public TaskService(AutoOpsDbContext context)
    {
        _context = context;
    }

    public List<TaskItem> GetAll()
    {
        return _context.Tasks
            .Where(t => !t.IsDeleted)
            .OrderByDescending(t => t.Id)
            .ToList();
    }

    public TaskItem Create(CreateTaskDto dto)
    {
        var task = new TaskItem
        {
            Title = dto.Title,
            IsCompleted = false,
            IsDeleted = false
        };

        _context.Tasks.Add(task);
        _context.SaveChanges();

        return task;
    }

    public void MarkAsCompleted(int id)
    {
        var task = _context.Tasks
            .FirstOrDefault(t => t.Id == id && !t.IsDeleted);

        if (task == null)
            throw new KeyNotFoundException("Task not found");

        if (task.IsCompleted)
            throw new InvalidOperationException("Task is already completed");

        task.IsCompleted = true;
        _context.SaveChanges();
    }

    public void SoftDelete(int id)
    {
        var task = _context.Tasks.FirstOrDefault(t => t.Id == id);

        if (task == null)
            throw new KeyNotFoundException("Task not found");

        task.IsDeleted = true;
        _context.SaveChanges();
    }
}
