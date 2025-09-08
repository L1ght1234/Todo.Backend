using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Todo_Backend.BLL.DTOs.Todos;
using Todo_Backend.DAL.Entities;
using Todo_Backend.DAL.Repositories;

namespace Todo_Backend.BLL.Commands.Todos.Create;
public class CreateTodoHandler : IRequestHandler<CreateTodoCommand, Result<GetTodoResponse>>
{
    private readonly IMapper _mapper;
    private readonly ITodoRepository _todoRepository;
    private readonly ILogger<CreateTodoHandler> _logger;

    public CreateTodoHandler(IMapper mapper, ITodoRepository todoRepository, ILogger<CreateTodoHandler> logger)
    {
        _mapper = mapper;
        _todoRepository = todoRepository;
        _logger = logger;
    }
    public async Task<Result<GetTodoResponse>> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        var newTodo = _mapper.Map<Todo>(request.CreateTodoRequest);

        if (newTodo is null)
        {
            var errorMsg = "CannotConvertNullToNews";
            _logger.LogError(errorMsg);
            return Result.Fail(errorMsg);
        }

        await _todoRepository.AddAsync(newTodo, cancellationToken);

        var todoRsponse = _mapper.Map<GetTodoResponse>(newTodo);

        return Result.Ok(todoRsponse);
    }
}