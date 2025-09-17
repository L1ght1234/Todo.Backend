using Todo_Backend.DAL.Enums;

namespace Todo_Backend.BLL.DTOs.Todos;
public record CreateTodoRequest(
    string Title,
    DateTime? Deadline,
    Status Status);