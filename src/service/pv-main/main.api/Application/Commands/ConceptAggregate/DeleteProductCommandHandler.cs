using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.Core.Aggregates.ConceptAggregate;
using OpenRIMS.PV.Main.Core.Exceptions;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.ConceptAggregate
{
    public class DeleteProductCommandHandler
        : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IRepositoryInt<Product> _productRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<DeleteProductCommandHandler> _logger;

        public DeleteProductCommandHandler(
            IRepositoryInt<Product> productRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<DeleteProductCommandHandler> logger)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(DeleteProductCommand message, CancellationToken cancellationToken)
        {
            var productFromRepo = await _productRepository.GetAsync(p => p.Id == message.ProductId, new string[] {
                "ConditionMedications",
                "PatientMedications"
            });
            if (productFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate product {message.ProductId}");
            }

            if (productFromRepo.ConditionMedications.Count() > 0 || productFromRepo.PatientMedications.Count() > 0)
            {
                throw new DomainException("Unable to delete product as it is currently in use");
            }

            _productRepository.Delete(productFromRepo);

            _logger.LogInformation($"----- Product {message.ProductId} deleted");

            return await _unitOfWork.CompleteAsync();
        }
    }
}
