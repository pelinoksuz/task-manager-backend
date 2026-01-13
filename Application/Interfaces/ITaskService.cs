using task_manager_backend.Domain.Entities;
using task_manager_backend.Application.DTOs;

namespace task_manager_backend.Application.Interfaces;

public interface ITaskService
{
    List<TaskItem> GetAll();
    TaskItem Create(CreateTaskDto dto);

    void MarkAsCompleted(int id);
    void SoftDelete(int id);
}
