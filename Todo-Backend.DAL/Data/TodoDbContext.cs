using Microsoft.EntityFrameworkCore;
using Todo_Backend.DAL.Entities;

namespace Todo_Backend.DAL.Data;
public class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    public DbSet<Todo> Todos { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(TodoDbContext).Assembly);
    }
}