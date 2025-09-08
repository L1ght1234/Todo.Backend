using FluentResults;
using MediatR;
using Todo_Backend.BLL.DTOs.Todos;

namespace Todo_Backend.BLL.Commands.Todos.Create;
public record CreateTodoCommand(CreateTodoRequest CreateTodoRequest) : IRequest<Result<GetTodoResponse>>;