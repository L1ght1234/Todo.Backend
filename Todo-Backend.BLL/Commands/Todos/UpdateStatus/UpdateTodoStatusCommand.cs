using FluentResults;
using MediatR;
using Todo_Backend.BLL.DTOs.Todos;
using Todo_Backend.DAL.Enums;

namespace Todo_Backend.BLL.Commands.Todos.UpdateStatus;
public record UpdateTodoStatusCommand(Guid Id, UpdateTodoStatusRequest UpdateTodoStatusRequest) : IRequest<Result<GetTodoResponse>>;