using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Commands.CustomAttributeAggregate
{
    public class DeleteSelectionValueCommandHandler
        : IRequestHandler<DeleteSelectionValueCommand, bool>
    {
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<DeleteSelectionValueCommandHandler> _logger;

        public DeleteSelectionValueCommandHandler(
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<SelectionDataItem> selectionDataItemRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<DeleteSelectionValueCommandHandler> logger)
        {
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(DeleteSelectionValueCommand message, CancellationToken cancellationToken)
        {
            var customAttributeFromRepo = await _customAttributeRepository.GetAsync(ca => ca.Id == message.CustomAttributeId, new string[] { "" });
            if (customAttributeFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate custom attribute");
            }

            var selectionDataItemFromRepo = await _selectionDataItemRepository.GetAsync(s => s.AttributeKey == customAttributeFromRepo.AttributeKey && s.SelectionKey == message.Key, new string[] { "" });
            if (selectionDataItemFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate selection data item");
            }

            _selectionDataItemRepository.Delete(selectionDataItemFromRepo);

            _logger.LogInformation($"----- Selection Data Item {message.Key} deleted");

            return await _unitOfWork.CompleteAsync();
        }
    }
}
