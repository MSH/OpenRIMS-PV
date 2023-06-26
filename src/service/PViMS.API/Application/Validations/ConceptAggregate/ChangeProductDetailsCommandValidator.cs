using FluentValidation;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Commands.ConceptAggregate;

namespace PVIMS.API.Application.Validations
{
    public class ChangeProductDetailsCommandValidator : AbstractValidator<ChangeProductDetailsCommand>
    {
        public ChangeProductDetailsCommandValidator(ILogger<ChangeProductDetailsCommandValidator> logger)
        {
            RuleFor(command => command.ProductName)
                .NotEmpty()
                .Length(1, 200)
                .Matches(@"[-a-zA-Z0-9 .;,()%]")
                .WithMessage("Product name contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen, period, semi-colon, comma, brackets, percentage)");

            RuleFor(command => command.Manufacturer)
                .NotEmpty()
                .Length(1, 200)
                .Matches(@"[-a-zA-Z0-9 .,()%]")
                .WithMessage("Manufacturer contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen, period, comma, brackets, percentage)");

            RuleFor(command => command.Description)
                .Length(0, 1000)
                .Matches(@"[-a-zA-Z0-9 .,()%]")
                .WithMessage("Description contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen, period, comma, brackets, percentage)");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
