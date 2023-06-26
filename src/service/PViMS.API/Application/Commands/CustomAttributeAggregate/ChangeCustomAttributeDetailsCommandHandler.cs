using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.Core.Models;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Commands.CustomAttributeAggregate
{
    public class ChangeCustomAttributeDetailsCommandHandler
        : IRequestHandler<ChangeCustomAttributeDetailsCommand, bool>
    {
        private readonly ICustomAttributeService _customAttributeService;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ChangeCustomAttributeDetailsCommandHandler> _logger;

        public ChangeCustomAttributeDetailsCommandHandler(
            ICustomAttributeService customAttributeService,
            IUnitOfWorkInt unitOfWork,
            ILogger<ChangeCustomAttributeDetailsCommandHandler> logger)
        {
            _customAttributeService = customAttributeService ?? throw new ArgumentNullException(nameof(customAttributeService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangeCustomAttributeDetailsCommand message, CancellationToken cancellationToken)
        {
            var detail = new CustomAttributeConfigDetail()
            {
                EntityName = message.ExtendableTypeName,
                AttributeName = message.AttributeKey,
                AttributeDetail = message.AttributeDetail,
                Category = message.Category,
                Required = message.IsRequired,
                Searchable = message.IsSearchable,
                NumericMaxValue = message.MaxValue,
                NumericMinValue = message.MinValue,
                FutureDateOnly = message.FutureDateOnly.HasValue ? message.FutureDateOnly.Value : false,
                PastDateOnly = message.PastDateOnly.HasValue ? message.PastDateOnly.Value : false,
                StringMaxLength = message.MaxLength
            };

            await _customAttributeService.UpdateCustomAttributeAsync(detail);

            _logger.LogInformation($"----- Custom Attribute {message.Id} details updated");

            return await _unitOfWork.CompleteAsync();
        }
    }
}
