using FluentValidation;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Commands.AppointmentAggregate;
using System;

namespace PVIMS.API.Application.Validations
{
    public class AddAppointmentCommandValidator : AbstractValidator<AddAppointmentCommand>
    {
        public AddAppointmentCommandValidator(ILogger<AddAppointmentCommandValidator> logger)
        {
            RuleFor(command => command.AppointmentDate)
                .GreaterThanOrEqualTo(p => DateTime.Now.Date)
                .WithMessage("Appointment Date should be after current date")
                .LessThanOrEqualTo(p => DateTime.Now.Date.AddYears(2))
                .WithMessage("Appointment Date should be within 2 years");

            RuleFor(command => command.Reason)
                .NotEmpty()
                .Length(0, 250)
                .Matches(@"[a-zA-Z0-9 ]")
                .WithMessage("Reason contains invalid characters (Enter A-Z, a-z, 0-9, space)");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
