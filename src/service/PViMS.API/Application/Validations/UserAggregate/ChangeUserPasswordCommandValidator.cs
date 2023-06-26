using FluentValidation;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Commands.UserAggregate;

namespace PVIMS.API.Application.Validations
{
    public class ChangeUserPasswordCommandValidator : AbstractValidator<ChangeUserPasswordCommand>
    {
        public ChangeUserPasswordCommandValidator(ILogger<ChangeUserPasswordCommandValidator> logger)
        {
            RuleFor(command => command.Password)
                .NotEmpty()
                .Length(1, 100);

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
