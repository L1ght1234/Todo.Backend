using Microsoft.AspNetCore.Mvc;
using Todo_Backend.BLL.Commands.Todos.Create;
using Todo_Backend.BLL.Commands.Todos.Delete;
using Todo_Backend.BLL.Commands.Todos.UpdateStatus;
using Todo_Backend.BLL.DTOs.Todos;
using Todo_Backend.BLL.Queries.Todos.GetAll;

namespace Todo_Backend.Controllers.Todos;

[ApiController]
[Route("todos")]
public class TodoController : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<List<GetTodoResponse>>> GetAllTodos(CancellationToken cancellationToken)
    {
        return HandleResult(await Mediator.Send(new GetAllTodosQuery(), cancellationToken));
    }
    [HttpPost]
    public async Task<ActionResult<GetTodoResponse>> CreateTodo(
        [FromBody] CreateTodoRequest createTodoRequest, CancellationToken cancellationToken)
    {
        return HandleResult(await Mediator.Send(new CreateTodoCommand(createTodoRequest), cancellationToken));
    }
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<string>> DeleteTodo(
        [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        return HandleResult(await Mediator.Send(new DeleteTodoCommand(id), cancellationToken));
    }
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<GetTodoResponse>> EditTodoStatus(
        [FromRoute] Guid id, [FromBody] UpdateTodoStatusRequest updateTodoStatusRequest, CancellationToken cancellationToken)
    {
        return HandleResult(await Mediator.Send(new UpdateTodoStatusCommand(id, updateTodoStatusRequest), cancellationToken));
    }
}