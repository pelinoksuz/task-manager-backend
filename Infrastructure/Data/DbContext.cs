using Microsoft.EntityFrameworkCore;
using task_manager_backend.Domain.Entities;

namespace task_manager_backend.Infrastructure.Data;

public class AutoOpsDbContext : DbContext
{
    public AutoOpsDbContext(DbContextOptions<AutoOpsDbContext> options)
        : base(options)
    {
    }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();
}
