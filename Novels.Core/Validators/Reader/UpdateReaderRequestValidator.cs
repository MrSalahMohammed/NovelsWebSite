using FluentValidation;
using Novels.Core.DTOs.Reader;

namespace Novels.Core.Validators.Reader
{
    public class UpdateReaderRequestValidator : AbstractValidator<UpdateReaderRequest>
    {
        public UpdateReaderRequestValidator()
        {
            RuleFor(x => x.FName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.RecoveryEmail).EmailAddress().When(x => x.RecoveryEmail is not null);
        }
    }
}
