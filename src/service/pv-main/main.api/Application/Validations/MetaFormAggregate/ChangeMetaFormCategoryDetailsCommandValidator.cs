using FluentValidation;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Application.Commands.MetaFormAggregate;

namespace OpenRIMS.PV.Main.API.Application.Validations
{
    public class ChangeMetaFormCategoryDetailsCommandValidator : AbstractValidator<ChangeMetaFormCategoryDetailsCommand>
    {
        public ChangeMetaFormCategoryDetailsCommandValidator(ILogger<ChangeMetaFormCategoryDetailsCommandValidator> logger)
        {
            RuleFor(command => command.CategoryName)
                .NotEmpty()
                .Length(1, 150)
                .Matches(@"[a-zA-Z0-9 ']")
                .WithMessage("Form name category contains invalid characters (Enter A-Z, a-z, 0-9, space, apostrophe)");

            RuleFor(command => command.Help)
                .Length(0, 500)
                .Matches(@"[-a-zA-Z0-9 .,()']")
                .When(c => !string.IsNullOrEmpty(c.Help))
                .WithMessage("Help contains invalid characters (Enter A-Z, a-z, 0-9, hyphen, space, period, comma, parentheses, apostrophe)");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
