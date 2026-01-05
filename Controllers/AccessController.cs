using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/access")]
public class AccessController : ControllerBase
{
    // GET /api/access?role=admin
    [HttpGet]
    public IActionResult Get([FromQuery] string? role)
    {
        var normalizedRole = role?.ToLower() ?? "guest";
        var response = GetAccessByRole(normalizedRole);
        return Ok(response);
    }

    // POST /api/access/login
    [HttpPost("login")]
    public IActionResult Login(LoginRequestDto request)
    {
        var users = new Dictionary<string, string>
        {
            { "admin", "1111" },
            { "operator", "2222" },
            { "user", "3333" }
        };

        if (string.IsNullOrWhiteSpace(request.Role) ||
            string.IsNullOrWhiteSpace(request.AccessCode))
        {
            return BadRequest("Role and access code are required");
        }

        var normalizedRole = request.Role.ToLower();

        if (!users.ContainsKey(normalizedRole))
            return Unauthorized("Invalid role");

        if (users[normalizedRole] != request.AccessCode)
            return Unauthorized("Invalid access code");

        return Ok(GetAccessByRole(normalizedRole));
    }

    // 🔒 Single source of truth for role → permission mapping
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
