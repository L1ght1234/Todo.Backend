using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Todo_Backend.BLL.Constants;
using Todo_Backend.DAL.Repositories;

namespace Todo_Backend.BLL.Commands.Todos.Delete;
public class DeleteTodoHandler : IRequestHandler<DeleteTodoCommand, Result<string>>
{
    private readonly ITodoRepository _todoRepository;
    private readonly ILogger<DeleteTodoHandler> _logger;

    public DeleteTodoHandler(ITodoRepository todoRepository, ILogger<DeleteTodoHandler> logger)
    {
        _todoRepository = todoRepository;
        _logger = logger;
    }
    public async Task<Result<string>> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        var result = await _todoRepository.DeleteAsync(request.Id, cancellationToken);

        if (!result)
        {
            var errorMsg = TodoConstants.DeleteTodoError;
            _logger.LogError(errorMsg);
            return Result.Fail(errorMsg);
        }

        return Result.Ok(TodoConstants.OperationSuccess);
    }
}