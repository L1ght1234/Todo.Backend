using Todo_Backend.DAL.Enums;

namespace Todo_Backend.BLL.DTOs.Todos;
public record CreateTodoRequest(
    string Title,
    string? Deadline,
    Status Status);