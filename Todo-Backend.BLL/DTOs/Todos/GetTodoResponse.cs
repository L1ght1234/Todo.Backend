
using Todo_Backend.DAL.Enums;

namespace Todo_Backend.BLL.DTOs.Todos;
public record GetTodoResponse(
    Guid id,
    string Title,
    DateTime? Deadline,
    Status Status);