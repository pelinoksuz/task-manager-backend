using Microsoft.AspNetCore.Mvc;
using task_manager_backend.Application.DTOs;

namespace task_manager_backend.Controllers;

/// <summary>
/// Access Control API Controller - Handles authentication and authorization
/// </summary>
[ApiController]
[Route("api/access")]
[Produces("application/json")]
public class AccessController : ControllerBase
{
    private readonly ILogger<AccessController> _logger;

    public AccessController(ILogger<AccessController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get access permissions by role
    /// </summary>
    /// <param name="role">User role (admin, operator, user, guest)</param>
    /// <returns>Access permissions for the role</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<AccessResponseDto>), 200)]
    public IActionResult Get([FromQuery] string? role)
    {
        try
        {
            var normalizedRole = role?.ToLower() ?? "guest";
            var response = GetAccessByRole(normalizedRole);
            return Ok(ApiResponse<AccessResponseDto>.SuccessResponse(response, "Access permissions retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving access permissions for role {Role}", role);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while retrieving access permissions"));
        }
    }

    /// <summary>
    /// Login with role and access code
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Access permissions if login successful</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AccessResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public IActionResult Login([FromBody] LoginRequestDto request)
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
            var users = new Dictionary<string, string>
            {
                { "admin", "1111" },
                { "operator", "2222" },
                { "user", "3333" }
            };

            var normalizedRole = request.Role.ToLower();

            if (!users.ContainsKey(normalizedRole))
            {
                _logger.LogWarning("Login attempt with invalid role: {Role}", request.Role);
                return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid role"));
            }

            if (users[normalizedRole] != request.AccessCode)
            {
                _logger.LogWarning("Login attempt with invalid access code for role: {Role}", request.Role);
                return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid access code"));
            }

            var response = GetAccessByRole(normalizedRole);
            _logger.LogInformation("Successful login for role: {Role}", request.Role);
            return Ok(ApiResponse<AccessResponseDto>.SuccessResponse(response, "Login successful"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred during login"));
        }
    }

    /// <summary>
    /// Single source of truth for role → permission mapping
    /// </summary>
    private AccessResponseDto GetAccessByRole(string role)
    {
        return role switch
        {
            "admin" => new AccessResponseDto
            {
                Role = "ADMIN",
                Permissions = new Dictionary<string, string>
                {
                    { "dashboard", "WRITE" },
                    { "vehicles", "WRITE" },
                    { "activities", "READ" }
                }
            },

            "operator" => new AccessResponseDto
            {
                Role = "OPERATOR",
                Permissions = new Dictionary<string, string>
                {
                    { "dashboard", "READ" },
                    { "vehicles", "WRITE" },
                    { "activities", "NONE" }
                }
            },

            "user" => new AccessResponseDto
            {
                Role = "USER",
                Permissions = new Dictionary<string, string>
                {
                    { "dashboard", "READ" },
                    { "vehicles", "READ" },
                    { "activities", "NONE" }
                }
            },

            _ => new AccessResponseDto
            {
                Role = "GUEST",
                Permissions = new Dictionary<string, string>
                {
                    { "dashboard", "READ" },
                    { "vehicles", "NONE" },
                    { "activities", "NONE" }
                }
            }
        };
    }
}
