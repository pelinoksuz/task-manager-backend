using System.ComponentModel.DataAnnotations;

namespace task_manager_backend.Application.DTOs;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Role is required")]
    public string Role { get; set; } = string.Empty;

    [Required(ErrorMessage = "Access code is required")]
    public string AccessCode { get; set; } = string.Empty;
}
