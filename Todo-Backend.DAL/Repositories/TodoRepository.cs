using Microsoft.EntityFrameworkCore;
using Todo_Backend.DAL.Data;
using Todo_Backend.DAL.Entities;
using Todo_Backend.DAL.Enums;

namespace Todo_Backend.DAL.Repositories;
public class TodoRepository : ITodoRepository
{
    private readonly TodoDbContext _context;

    public TodoRepository(TodoDbContext context)
    {
        _context = context;
    }

    public async Task<List<Todo>> GetAsync(CancellationToken cancellationToken)
    {
        return await _context.Todos
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Todo> AddAsync(Todo todo, CancellationToken cancellationToken)
    {
        _context.Todos.Add(todo);
        await _context.SaveChangesAsync(cancellationToken);
        return todo;
    }

    public async Task<Todo?> UpdateStatusAsync(Guid id, Status newStatus, CancellationToken cancellationToken)
    {
        var existing = await _context.Todos.FindAsync(new object[] { id }, cancellationToken);
        if (existing == null)
            return null;

        existing.Status = newStatus;

        await _context.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var todo = await _context.Todos.FindAsync(id, cancellationToken);
        if (todo == null)
            return false;

        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}