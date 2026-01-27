using Microsoft.AspNetCore.Mvc;
using task_manager_backend.Application.Interfaces;
using task_manager_backend.Application.DTOs;

namespace task_manager_backend.Controllers;

/// <summary>
/// Tasks API Controller - Manages task CRUD operations
/// </summary>
[ApiController]
[Route("api/tasks")]
[Produces("application/json")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITaskService taskService, ILogger<TasksController> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    /// <summary>
    /// Get all tasks (excluding deleted ones)
    /// </summary>
    /// <returns>List of tasks</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<Domain.Entities.TaskItem>>), 200)]
    public IActionResult GetAll()
    {
        try
        {
            var tasks = _taskService.GetAll();
            return Ok(ApiResponse<List<Domain.Entities.TaskItem>>.SuccessResponse(tasks, "Tasks retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while retrieving tasks"));
        }
    }

    /// <summary>
    /// Create a new task
    /// </summary>
    /// <param name="dto">Task creation data</param>
    /// <returns>Created task</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Domain.Entities.TaskItem>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public IActionResult Create([FromBody] CreateTaskDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(ApiResponse<object>.ErrorResponse("Validation failed", errors));
        }

        try
        {
            var createdTask = _taskService.Create(dto);
            return CreatedAtAction(
                nameof(GetAll),
                new { id = createdTask.Id },
                ApiResponse<Domain.Entities.TaskItem>.SuccessResponse(createdTask, "Task created successfully")
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while creating the task"));
        }
    }

    /// <summary>
    /// Mark a task as completed
    /// </summary>
    /// <param name="id">Task ID</param>
    [HttpPatch("{id}/complete")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public IActionResult Complete(int id)
    {
        try
        {
            _taskService.MarkAsCompleted(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing task {TaskId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while completing the task"));
        }
    }

    /// <summary>
    /// Soft delete a task
    /// </summary>
    /// <param name="id">Task ID</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public IActionResult Delete(int id)
    {
        try
        {
            _taskService.SoftDelete(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting task {TaskId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while deleting the task"));
        }
    }
}
