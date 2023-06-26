using FluentValidation;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Queries.EncounterAggregate;

namespace PVIMS.API.Application.Validations
{
    public class EncountersDetailQueryValidator : AbstractValidator<EncountersDetailQuery>
    {
        public EncountersDetailQueryValidator(ILogger<EncountersDetailQueryValidator> logger)
        {
            RuleFor(query => query.FirstName)
                .Matches(@"[-a-zA-Z ']")
                .When(v => !string.IsNullOrEmpty(v.FirstName));

            RuleFor(query => query.LastName)
                .Matches(@"[-a-zA-Z ']")
                .When(v => !string.IsNullOrEmpty(v.LastName));

            RuleFor(query => query.CustomAttributeValue)
                .Matches(@"[-a-zA-Z0-9 ']")
                .When(v => !string.IsNullOrEmpty(v.CustomAttributeValue));

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
