using FluentValidation;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Commands.CustomAttributeAggregate;

namespace PVIMS.API.Application.Validations
{
    public class AddCustomAttributeCommandValidator : AbstractValidator<AddCustomAttributeCommand>
    {
        public AddCustomAttributeCommandValidator(ILogger<AddFacilityCommandValidator> logger)
        {
            RuleFor(command => command.ExtendableTypeName)
                .NotEmpty()
                .Length(1, 450)
                .Matches(@"[a-zA-Z]")
                .WithMessage("Extendable type name contains invalid characters (Enter A-Z, a-z)");

            RuleFor(command => command.AttributeKey)
                .NotEmpty()
                .Length(1, 450)
                .Matches(@"[a-zA-Z]")
                .WithMessage("Attribute key contains invalid characters (Enter A-Z, a-z)");

            RuleFor(command => command.AttributeDetail)
                .Length(0, 150)
                .Matches(@"[a-zA-Z]")
                .WithMessage("Attribute detail contains invalid characters (Enter A-Z, a-z)");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
