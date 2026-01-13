using Microsoft.AspNetCore.Mvc;
using task_manager_backend.Application.Interfaces;
using task_manager_backend.Application.DTOs;

namespace task_manager_backend.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    // 1️⃣ GET /api/tasks
    [HttpGet]
    public IActionResult GetAll()
    {
        var tasks = _taskService.GetAll();
        return Ok(tasks);
    }

    // 2️⃣ POST /api/tasks
    [HttpPost]
    public IActionResult Create([FromBody] CreateTaskDto dto)
    {
        var createdTask = _taskService.Create(dto);

        // RESTful yaklaşım
        return CreatedAtAction(
            nameof(GetAll),
            new { id = createdTask.Id },
            createdTask
        );
    }

    // 3️⃣ PATCH /api/tasks/{id}/complete
    [HttpPatch("{id}/complete")]
    public IActionResult Complete(int id)
    {
        _taskService.MarkAsCompleted(id);
        return NoContent();
    }

    // 4️⃣ DELETE /api/tasks/{id}
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _taskService.SoftDelete(id);
        return NoContent();
    }
}
