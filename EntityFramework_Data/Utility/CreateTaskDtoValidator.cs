using FluentValidation;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Utility
{
    public class CreateTaskDtoValidator : AbstractValidator<CreateOrUpdateTaskDto>
    {
        public CreateTaskDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
