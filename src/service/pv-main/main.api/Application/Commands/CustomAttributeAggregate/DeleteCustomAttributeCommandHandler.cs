using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.CustomAttributeAggregate
{
    public class DeleteCustomAttributeCommandHandler
        : IRequestHandler<DeleteCustomAttributeCommand, bool>
    {
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<DeleteCustomAttributeCommandHandler> _logger;

        public DeleteCustomAttributeCommandHandler(
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<DeleteCustomAttributeCommandHandler> logger)
        {
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(DeleteCustomAttributeCommand message, CancellationToken cancellationToken)
        {
            var customAttributeFromRepo = await _customAttributeRepository.GetAsync(ca => ca.Id == message.Id, new string[] { "" });
            if (customAttributeFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate custom attribute");
            }

            _customAttributeRepository.Delete(customAttributeFromRepo);

            _logger.LogInformation($"----- Custom Attribute {message.Id} deleted");

            return await _unitOfWork.CompleteAsync();
        }
    }
}
