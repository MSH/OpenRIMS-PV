using FluentValidation;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Application.Commands.ContactAggregate;

namespace OpenRIMS.PV.Main.API.Application.Validations
{
    public class ChangeContactDetailsCommandValidator : AbstractValidator<ChangeContactDetailsCommand>
    {
        public ChangeContactDetailsCommandValidator(ILogger<ChangeContactDetailsCommandValidator> logger)
        {
            RuleFor(command => command.OrganisationName)
                .NotEmpty()
                .Length(1, 60)
                .Matches(@"[a-zA-Z0-9 ]")
                .WithMessage("Organisation name contains invalid characters (Enter A-Z, a-z, 0-9, space)");

            RuleFor(command => command.DepartmentName)
                .NotEmpty()
                .Length(1, 60)
                .Matches(@"[a-zA-Z0-9 ]")
                .WithMessage("Department name contains invalid characters (Enter A-Z, a-z, 0-9, space)");

            RuleFor(command => command.ContactFirstName)
                .NotEmpty()
                .Length(1, 35)
                .Matches(@"[a-zA-Z ]")
                .WithMessage("Contact first name contains invalid characters (Enter A-Z, a-z, space)");

            RuleFor(command => command.ContactSurname)
                .NotEmpty()
                .Length(1, 35)
                .Matches(@"[a-zA-Z ]")
                .WithMessage("Contact surname contains invalid characters (Enter A-Z, a-z, space)");

            RuleFor(command => command.StreetAddress)
                .NotEmpty()
                .Length(1, 100)
                .Matches(@"[a-zA-Z0-9 ']")
                .WithMessage("Street address contains invalid characters(Enter A-Z, a-z, 0-9, space, apostrophe)");

            RuleFor(command => command.City)
                .NotEmpty()
                .Length(1, 50)
                .Matches(@"[a-zA-Z ]")
                .WithMessage("City contains invalid characters (Enter A-Z, a-z, space)");

            RuleFor(command => command.State)
                .Length(0, 50)
                .Matches(@"[a-zA-Z ]")
                .When(c => !string.IsNullOrEmpty(c.State))
                .WithMessage("State contains invalid characters (Enter A-Z, a-z, space)");

            RuleFor(command => command.PostCode)
                .Length(0, 20)
                .Matches(@"[a-zA-Z0-9]")
                .When(c => !string.IsNullOrEmpty(c.PostCode))
                .WithMessage("Post code contains invalid characters (Enter A-Z, a-z, 0-9)");

            RuleFor(command => command.CountryCode)
                .Length(0, 10)
                .Matches(@"[0-9]")
                .When(c => !string.IsNullOrEmpty(c.CountryCode))
                .WithMessage("Country code contains invalid characters (Enter 0-9)");

            RuleFor(command => command.ContactNumber)
                .Length(0, 50)
                .Matches(@"[-0-9]")
                .When(c => !string.IsNullOrEmpty(c.ContactNumber))
                .WithMessage("Contact number contains invalid characters (Enter hyphen, 0-9)");

            RuleFor(command => command.ContactEmail)
                .Length(0, 50)
                .Matches(@"[-a-zA-Z@._]")
                .When(c => !string.IsNullOrEmpty(c.ContactEmail))
                .WithMessage("Contact email contains invalid characters (Enter A-Z, a-z, hyphen, @, period, underscore)");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
