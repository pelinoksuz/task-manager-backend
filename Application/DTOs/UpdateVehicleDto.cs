using System.ComponentModel.DataAnnotations;

namespace task_manager_backend.Application.DTOs;

public class UpdateVehicleDto
{
    [Required(ErrorMessage = "Vehicle name is required")]
    public string VehicleName { get; set; } = string.Empty;
    
    public string Model { get; set; } = string.Empty;
    public string Project { get; set; } = string.Empty;
    public string Health { get; set; } = "OK";
    public int Scc { get; set; }
    public int Temp { get; set; }
    public int Torque { get; set; }
    public int Speed { get; set; }
    public string Updated { get; set; } = "just now";
}
