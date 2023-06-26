using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Aggregates.ConceptAggregate;
using PVIMS.Core.Exceptions;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Commands.ConceptAggregate
{
    public class AddProductCommandHandler
        : IRequestHandler<AddProductCommand, ProductIdentifierDto>
    {
        private readonly IRepositoryInt<Concept> _conceptRepository;
        private readonly IRepositoryInt<Product> _productRepository;
        private readonly IUnitOfWorkInt _unitOfWork; 
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<AddProductCommandHandler> _logger;

        public AddProductCommandHandler(
            IRepositoryInt<Concept> conceptRepository,
            IRepositoryInt<Product> productRepository,
            IUnitOfWorkInt unitOfWork,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<AddProductCommandHandler> logger)
        {
            _conceptRepository = conceptRepository ?? throw new ArgumentNullException(nameof(conceptRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ProductIdentifierDto> Handle(AddProductCommand message, CancellationToken cancellationToken)
        {
            var conceptFromRepo = await _conceptRepository.GetAsync(c => c.ConceptName + "; " + c.Strength + " (" + c.MedicationForm.Description + ")" == message.ConceptName);
            if (conceptFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate concept {message.ConceptName}");
            }

            if (_productRepository.Exists(p => p.ConceptId == conceptFromRepo.Id &&
                p.ProductName == message.ProductName))
            {
                throw new DomainException("Product with same name annd concept already exists");
            }

            var newProduct = conceptFromRepo.AddProduct(message.ProductName,
                message.Manufacturer,
                message.Description);

            _conceptRepository.Update(conceptFromRepo);

            _logger.LogInformation($"----- Product {message.ProductName} created");

            await _unitOfWork.CompleteAsync();

            var mappedProduct = _mapper.Map<ProductIdentifierDto>(newProduct);

            CreateLinks(mappedProduct);

            return mappedProduct;
        }

        private void CreateLinks(ProductIdentifierDto dto)
        {
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Product", dto.Id), "self", "GET"));
        }
    }
}
