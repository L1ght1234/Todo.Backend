using Todo_Backend.DAL.Entities;
using Todo_Backend.DAL.Enums;

namespace Todo_Backend.DAL.Repositories;
public interface ITodoRepository
{
    Task<List<Todo>> GetAsync(CancellationToken cancellationToken);
    Task<Todo> AddAsync(Todo todo, CancellationToken cancellationToken);
    Task<Todo?> UpdateStatusAsync(Guid id, Status newStatus, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}