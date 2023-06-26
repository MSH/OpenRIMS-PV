using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.Core.Models;
using PVIMS.Core.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Commands.CustomAttributeAggregate
{
    public class AddSelectionValueCommandHandler
        : IRequestHandler<AddSelectionValueCommand, bool>
    {
        private readonly ICustomAttributeService _customAttributeService;
        private readonly ILogger<AddSelectionValueCommandHandler> _logger;

        public AddSelectionValueCommandHandler(
            ICustomAttributeService customAttributeService,
            ILogger<AddSelectionValueCommandHandler> logger)
        {
            _customAttributeService = customAttributeService ?? throw new ArgumentNullException(nameof(customAttributeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(AddSelectionValueCommand message, CancellationToken cancellationToken)
        {
            var detail = new SelectionDataItemDetail()
            {
                AttributeKey = message.AttributeKey,
                DataItemValue = message.DataItemValue,
                SelectionKey = message.SelectionKey,
            };

            await _customAttributeService.AddSelectionDataItemAsync(detail);

            _logger.LogInformation($"----- Selection Data Item {message.AttributeKey} created");

            return true;
        }
    }
}
