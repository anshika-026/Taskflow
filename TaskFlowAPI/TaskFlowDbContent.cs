using Microsoft.EntityFrameworkCore;

namespace TaskFlowAPI;

public class TaskFlowDbContext : DbContext
{
    public TaskFlowDbContext(DbContextOptions<TaskFlowDbContext> options) : base(options) { }

    public DbSet<TodoItem> Todos => Set<TodoItem>();
}