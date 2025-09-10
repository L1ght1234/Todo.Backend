using FluentValidation;
using Todo_Backend.BLL.DTOs.Todos;

namespace Todo_Backend.BLL.Validators.Todos;
public class CreateTodoRequestValidator : AbstractValidator<CreateTodoRequest>
{
    public CreateTodoRequestValidator()
    {
        RuleFor(e => e.Title)
            .NotEmpty().WithMessage("Title can`t be empty")
            .MaximumLength(200).WithMessage("Title can`t be than 200 characters");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid content type");
    }
}