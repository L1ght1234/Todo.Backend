using FluentResults;
using MediatR;
using Todo_Backend.BLL.DTOs.Todos;

namespace Todo_Backend.BLL.Queries.Todos.GetAll;
public record GetAllTodosQuery : IRequest<Result<List<GetTodoResponse>>>;