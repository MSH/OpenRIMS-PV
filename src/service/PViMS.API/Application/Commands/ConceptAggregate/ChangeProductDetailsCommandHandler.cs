using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.Core.Aggregates.ConceptAggregate;
using PVIMS.Core.Exceptions;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Commands.ConceptAggregate
{
    public class ChangeProductDetailsCommandHandler
        : IRequestHandler<ChangeProductDetailsCommand, bool>
    {
        private readonly IRepositoryInt<Concept> _conceptRepository;
        private readonly IRepositoryInt<Product> _productRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ChangeProductDetailsCommandHandler> _logger;

        public ChangeProductDetailsCommandHandler(
            IRepositoryInt<Concept> conceptRepository,
            IRepositoryInt<Product> productRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<ChangeProductDetailsCommandHandler> logger)
        {
            _conceptRepository = conceptRepository ?? throw new ArgumentNullException(nameof(conceptRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangeProductDetailsCommand message, CancellationToken cancellationToken)
        {
            var conceptFromRepo = await _conceptRepository.GetAsync(c => c.ConceptName + "; " + c.Strength + " (" + c.MedicationForm.Description + ")" == message.ConceptName, new string[] {
                "Products"
            });
            if (conceptFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate concept {message.ConceptName}");
            }

            if (_productRepository.Exists(p => p.ConceptId == conceptFromRepo.Id &&
                p.ProductName == message.ProductName &&
                p.Id != message.ProductId))
            {
                throw new DomainException("Product with same name annd concept already exists");
            }

            conceptFromRepo.ChangeProductDetails(message.ProductId, message.ProductName, message.Manufacturer, message.Description);
            if (message.Active)
            {
                conceptFromRepo.MarkProductAsActive(message.ProductId);
            }
            else
            {
                conceptFromRepo.MarkProductAsInActive(message.ProductId);
            }
            _conceptRepository.Update(conceptFromRepo);

            _logger.LogInformation($"----- Product {message.ProductId} details updated");

            return await _unitOfWork.CompleteAsync();
        }
    }
}