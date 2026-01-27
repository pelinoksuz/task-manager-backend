using Microsoft.AspNetCore.Mvc;
using task_manager_backend.Application.DTOs;

namespace task_manager_backend.Controllers;

/// <summary>
/// Health Check API Controller - Provides system health status
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Get health status of the API
    /// </summary>
    /// <returns>Health status</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public IActionResult Get()
    {
        return Ok(ApiResponse<object>.SuccessResponse(
            new { status = "ok", timestamp = DateTime.UtcNow },
            "API is healthy"
        ));
    }
}
