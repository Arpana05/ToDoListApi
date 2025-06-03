using FluentValidation;
using ToDoListApi.DTOs;

namespace ToDoListApi.Validators;

public class CreateItemDtoValidator : AbstractValidator<CreateItemDto>
{
    public CreateItemDtoValidator()
    {
        RuleFor(x => x.ItemTitle)
            .NotEmpty().WithMessage("Item title is required.")
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.Now).WithMessage("Due date must be in the future.");

        RuleFor(x => x.CategoryName)
            .NotEmpty().WithMessage("Category name is required.");
    }
}

public class UpdateItemDtoValidator : AbstractValidator<UpdateItemDto>
{
    public UpdateItemDtoValidator()
    {
        RuleFor(x => x.ItemTitle)
            .NotEmpty().WithMessage("Item title is required.")
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.Now).WithMessage("Due date must be in the future.");

        RuleFor(x => x.CategoryName)
            .NotEmpty().WithMessage("Category name is required.");
    }
}
