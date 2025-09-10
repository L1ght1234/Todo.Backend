using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Todo_Backend.BLL.Constants;
using Todo_Backend.BLL.DTOs.Todos;
using Todo_Backend.DAL.Repositories;

namespace Todo_Backend.BLL.Commands.Todos.UpdateStatus;
public class UpdateTodoStatusHandler : IRequestHandler<UpdateTodoStatusCommand, Result<GetTodoResponse>>
{
    private readonly IMapper _mapper;
    private readonly ITodoRepository _todoRepository;
    private readonly ILogger<UpdateTodoStatusHandler> _logger;
    private readonly IValidator<UpdateTodoStatusRequest> _validator;


    public UpdateTodoStatusHandler(IMapper mapper, 
        ITodoRepository todoRepository, 
        ILogger<UpdateTodoStatusHandler> logger,
        IValidator<UpdateTodoStatusRequest> validator)
    {
        _mapper = mapper;
        _todoRepository = todoRepository;
        _logger = logger;
        _validator = validator;
    }
    public async Task<Result<GetTodoResponse>> Handle(UpdateTodoStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request.UpdateTodoStatusRequest, cancellationToken);

            var result = await _todoRepository.UpdateStatusAsync(request.Id, request.UpdateTodoStatusRequest.Status, cancellationToken);

            if (result is null)
            {
                var errorMsg = TodoConstants.UpdateTodoError;
                _logger.LogError(errorMsg);
                return Result.Fail(errorMsg);
            }

            var todoRsponse = _mapper.Map<GetTodoResponse>(result);

            return Result.Ok(todoRsponse);
        }
        catch (ValidationException ex)
        {
            return Result.Fail<GetTodoResponse>(ex.Message);
        }
    }
}