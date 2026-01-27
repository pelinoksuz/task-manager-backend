using Microsoft.AspNetCore.Mvc;
using task_manager_backend.Application.DTOs;
using task_manager_backend.Domain.Entities;
using DashboardSummaryDto = task_manager_backend.Application.DTOs.DashboardSummaryDto;

namespace task_manager_backend.Controllers;

/// <summary>
/// Vehicles API Controller - Manages vehicle data
/// </summary>
[ApiController]
[Route("api/vehicles")]
[Produces("application/json")]
public class VehiclesController : ControllerBase
{
    private readonly ILogger<VehiclesController> _logger;
    private static List<Vehicle>? _vehiclesCache = null;
    private static readonly object _lock = new object();

    public VehiclesController(ILogger<VehiclesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all vehicles
    /// </summary>
    /// <returns>List of vehicles</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<Vehicle>>), 200)]
    public IActionResult GetAll()
    {
        try
        {
            var vehicles = GetVehicles();
            return Ok(ApiResponse<List<Vehicle>>.SuccessResponse(vehicles, "Vehicles retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vehicles");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while retrieving vehicles"));
        }
    }

    /// <summary>
    /// Get dashboard summary with vehicle statistics
    /// </summary>
    /// <returns>Dashboard summary with total, ok, warn, error counts and latest vehicles</returns>
    [HttpGet("dashboard-summary")]
    [ProducesResponseType(typeof(ApiResponse<DashboardSummaryDto>), 200)]
    public IActionResult GetDashboardSummary()
    {
        try
        {
            var vehicles = GetVehicles();
            var summary = new DashboardSummaryDto
            {
                Total = vehicles.Count,
                Ok = vehicles.Count(v => v.Health == "OK"),
                Warn = vehicles.Count(v => v.Health == "WARN"),
                Error = vehicles.Count(v => v.Health == "ERROR"),
                Latest = vehicles.Take(5).ToList()
            };

            return Ok(ApiResponse<DashboardSummaryDto>.SuccessResponse(summary, "Dashboard summary retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard summary");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while retrieving dashboard summary"));
        }
    }

    /// <summary>
    /// Get vehicle by ID
    /// </summary>
    /// <param name="id">Vehicle ID</param>
    /// <returns>Vehicle details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<Vehicle>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public IActionResult GetById(int id)
    {
        try
        {
            var vehicles = GetVehicles();
            var vehicle = vehicles.FirstOrDefault(v => v.Id == id);

            if (vehicle == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"Vehicle with ID {id} not found"));
            }

            return Ok(ApiResponse<Vehicle>.SuccessResponse(vehicle, "Vehicle retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vehicle {VehicleId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while retrieving vehicle"));
        }
    }

    /// <summary>
    /// Create a new vehicle
    /// </summary>
    /// <param name="dto">Vehicle creation data</param>
    /// <returns>Created vehicle</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Vehicle>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public IActionResult Create([FromBody] CreateVehicleDto dto)
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
            var newVehicle = new Vehicle
            {
                VehicleName = dto.VehicleName,
                Model = dto.Model,
                Project = dto.Project,
                Health = dto.Health,
                Scc = dto.Scc,
                Temp = dto.Temp,
                Torque = dto.Torque,
                Speed = dto.Speed,
                Updated = dto.Updated
            };

            lock (_lock)
            {
                // Ensure cache is populated (use InitializeVehiclesCache to avoid deadlock)
                if (_vehiclesCache == null)
                {
                    _vehiclesCache = InitializeVehiclesCache();
                }
                
                // Calculate ID inside the lock to prevent race conditions
                newVehicle.Id = _vehiclesCache.Count > 0 ? _vehiclesCache.Max(v => v.Id) + 1 : 1;
                
                // Add the vehicle to the cache
                _vehiclesCache.Add(newVehicle);
            }

            return CreatedAtAction(
                nameof(GetAll),
                new { id = newVehicle.Id },
                ApiResponse<Vehicle>.SuccessResponse(newVehicle, "Vehicle created successfully")
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating vehicle");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while creating the vehicle"));
        }
    }

    /// <summary>
    /// Update an existing vehicle
    /// </summary>
    /// <param name="id">Vehicle ID</param>
    /// <param name="dto">Vehicle update data</param>
    /// <returns>Updated vehicle</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<Vehicle>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public IActionResult Update(int id, [FromBody] UpdateVehicleDto dto)
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
            var vehicles = GetVehicles();
            var vehicle = vehicles.FirstOrDefault(v => v.Id == id);

            if (vehicle == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"Vehicle with ID {id} not found"));
            }

            lock (_lock)
            {
                vehicle.VehicleName = dto.VehicleName;
                vehicle.Model = dto.Model;
                vehicle.Project = dto.Project;
                vehicle.Health = dto.Health;
                vehicle.Scc = dto.Scc;
                vehicle.Temp = dto.Temp;
                vehicle.Torque = dto.Torque;
                vehicle.Speed = dto.Speed;
                // Updated field'ını otomatik olarak güncelle
                vehicle.Updated = "just now";
            }

            return Ok(ApiResponse<Vehicle>.SuccessResponse(vehicle, "Vehicle updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating vehicle {VehicleId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while updating the vehicle"));
        }
    }

    /// <summary>
    /// Delete a vehicle
    /// </summary>
    /// <param name="id">Vehicle ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public IActionResult Delete(int id)
    {
        try
        {
            var vehicles = GetVehicles();
            var vehicle = vehicles.FirstOrDefault(v => v.Id == id);

            if (vehicle == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"Vehicle with ID {id} not found"));
            }

            lock (_lock)
            {
                if (_vehiclesCache == null)
                {
                    _vehiclesCache = GetVehicles();
                }
                _vehiclesCache.Remove(vehicle);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting vehicle {VehicleId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while deleting the vehicle"));
        }
    }

    /// <summary>
    /// Generate or retrieve cached vehicles list
    /// </summary>
    private List<Vehicle> GetVehicles()
    {
        if (_vehiclesCache != null)
        {
            return _vehiclesCache;
        }

        lock (_lock)
        {
            if (_vehiclesCache != null)
            {
                return _vehiclesCache;
            }

            var random = new Random();
            var vehicleMakes = new[]
            {
                "Toyota", "Honda", "Ford", "Chevrolet", "BMW", "Mercedes-Benz", "Audi", "Volkswagen",
                "Nissan", "Hyundai", "Kia", "Mazda", "Subaru", "Lexus", "Acura", "Infiniti",
                "Volvo", "Jaguar", "Land Rover", "Porsche", "Tesla", "Genesis", "Cadillac", "Lincoln"
            };

            _vehiclesCache = new List<Vehicle>();
            var healthStatuses = new[] { "OK", "WARN", "ERROR" };

            for (int i = 0; i < 100; i++)
            {
                var make = vehicleMakes[random.Next(vehicleMakes.Length)];
                var health = healthStatuses[random.Next(healthStatuses.Length)];
                var minsAgo = random.Next(10000);
                var updated = $"{minsAgo / 60} min ago";

                _vehiclesCache.Add(new Vehicle
                {
                    Id = i + 1,
                    VehicleName = make,
                    Model = $"Model-{random.Next(100, 1000)}",
                    Project = $"Project {Convert.ToChar(65 + (i % 26))}",
                    Health = health,
                    Scc = random.Next(100),
                    Temp = random.Next(300),
                    Torque = random.Next(700),
                    Speed = random.Next(10),
                    Updated = updated
                });
            }

            return _vehiclesCache;
        }
    }
}
