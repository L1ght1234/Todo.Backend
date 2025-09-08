using FluentResults;
using MediatR;

namespace Todo_Backend.BLL.Commands.Todos.Delete;
public record DeleteTodoCommand(Guid Id) : IRequest<Result<string>>;