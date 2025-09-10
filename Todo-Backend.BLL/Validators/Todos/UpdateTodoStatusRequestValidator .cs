using FluentValidation;
using Todo_Backend.BLL.DTOs.Todos;

namespace Todo_Backend.BLL.Validators.Todos;
public class UpdateTodoStatusRequestValidator : AbstractValidator<UpdateTodoStatusRequest>
{
    public UpdateTodoStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid content type");
    }
}