using FluentValidation;
using Novels.Core.DTOs.Novel;

namespace Novels.Core.Validators.Novel
{
    public class NovelValidator : AbstractValidator<AddChapterRequest>
    {
        public NovelValidator()
        {
            RuleFor(c => c.Name).NotNull().NotEmpty().MaximumLength(300);
            RuleFor(c => c.Content).NotNull().NotEmpty();
        }
    }
}
