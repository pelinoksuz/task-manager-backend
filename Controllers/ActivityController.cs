using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using task_manager_backend.Application.DTOs;
using task_manager_backend.Domain.Entities;
using task_manager_backend.Infrastructure.Data;

namespace task_manager_backend.Controllers;

/// <summary>
/// Activity Logs API Controller - Manages activity logs
/// </summary>
[ApiController]
[Route("api/activities")]
[Produces("application/json")]
public class ActivityController : ControllerBase
{
    private readonly AutoOpsDbContext _context;
    private readonly ILogger<ActivityController> _logger;

    public ActivityController(AutoOpsDbContext context, ILogger<ActivityController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all activity logs
    /// </summary>
    /// <returns>List of activity logs</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<ActivityLogDto>>), 200)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var logs = await _context
                .ActivityLogs
                .OrderByDescending(a => a.Time)
                .Select(
                    a =>
                        new ActivityLogDto
                        {
                            Id = a.Id,
                            User = a.User,
                            Action = a.Action,
                            Time = a.Time.ToString("yyyy-MM-dd HH:mm:ss"),
                        }
                )
                .ToListAsync();

            return Ok(
                ApiResponse<List<ActivityLogDto>>.SuccessResponse(
                    logs,
                    "Activity logs retrieved successfully"
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving activity logs");
            return StatusCode(
                500,
                ApiResponse<object>.ErrorResponse(
                    "An error occurred while retrieving activity logs"
                )
            );
        }
    }

    /// <summary>
    /// Create a new activity log
    /// </summary>
    /// <param name="dto">Activity log data</param>
    /// <returns>Created activity log</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ActivityLogDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> Create([FromBody] CreateActivityLogDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(ApiResponse<object>.ErrorResponse("Validation failed", errors));
        }

        try
        {
            var activityLog = new ActivityLog
            {
                User = dto.User,
                Action = dto.Action,
                Time = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
            };

            _context.ActivityLogs.Add(activityLog);
            await _context.SaveChangesAsync();

            var response = new ActivityLogDto
            {
                Id = activityLog.Id,
                User = activityLog.User,
                Action = activityLog.Action,
                Time = activityLog.Time.ToString("yyyy-MM-dd HH:mm:ss")
            };

            return CreatedAtAction(
                nameof(GetAll),
                new { id = activityLog.Id },
                ApiResponse<ActivityLogDto>.SuccessResponse(response, "Activity log created successfully")
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating activity log");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while creating activity log"));
        }
    }

    /// <summary>
    /// Clear all activity logs
    /// </summary>
    [HttpDelete]
    [ProducesResponseType(204)]
    public async Task<IActionResult> ClearAll()
    {
        try
        {
            var logs = await _context.ActivityLogs.ToListAsync();
            _context.ActivityLogs.RemoveRange(logs);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing activity logs");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while clearing activity logs"));
        }
    }
}
