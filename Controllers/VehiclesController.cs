using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/vehicles")]
public class VehiclesController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        var vehicles = new List<Vehicle>
        {
            new Vehicle
            {
                Id = 1,
                VehicleName = "Vehicle A",
                Model = "Model-X",
                Project = "Project Alpha",
                Health = "OK",
                Scc = 95,
                Temp = 120,
                Torque = 450,
                Speed = 130,
                Updated = "just now"
            },
            new Vehicle
            {
                Id = 2,
                VehicleName = "Vehicle B",
                Model = "Model-Y",
                Project = "Project Beta",
                Health = "WARN",
                Scc = 80,
                Temp = 150,
                Torque = 380,
                Speed = 110,
                Updated = "2 min ago"
            }
        };

        return Ok(vehicles);
    }
}
