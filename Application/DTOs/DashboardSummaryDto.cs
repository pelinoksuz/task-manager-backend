using task_manager_backend.Domain.Entities;

namespace task_manager_backend.Application.DTOs;

public class DashboardSummaryDto
{
    public int Total { get; set; }
    public int Ok { get; set; }
    public int Warn { get; set; }
    public int Error { get; set; }
    public List<Vehicle> Latest { get; set; } = new List<Vehicle>();
}
