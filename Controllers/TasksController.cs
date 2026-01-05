using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private static List<TaskItem> _tasks = new()
    {
        new TaskItem { Id = 1, Title = "Learn backend basics", IsCompleted = false },
        new TaskItem { Id = 2, Title = "Connect frontend to API", IsCompleted = false }
    };

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_tasks);
    }

    [HttpPost]
    public IActionResult Create(CreateTaskDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            return BadRequest("Title is required");
        }

        var newTask = new TaskItem
        {
            Id = _tasks.Max(t => t.Id) + 1,
            Title = dto.Title,
            IsCompleted = false
        };

        _tasks.Add(newTask);

        return Ok(newTask);
    }
}
