using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Aggregates.ConceptAggregate;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Queries.ConceptAggregate
{
    public class ProductIdentifierQueryHandler
        : IRequestHandler<ProductIdentifierQuery, ProductIdentifierDto>
    {
        private readonly IRepositoryInt<Product> _productRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductIdentifierQueryHandler> _logger;

        public ProductIdentifierQueryHandler(
            IRepositoryInt<Product> productRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<ProductIdentifierQueryHandler> logger)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ProductIdentifierDto> Handle(ProductIdentifierQuery message, CancellationToken cancellationToken)
        {
            var productFromRepo = await _productRepository.GetAsync(p => p.Id == message.ProductId, new string[] { "Concept.MedicationForm" });

            if (productFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate product {message.ProductId}");
            }

            var mappedProduct = _mapper.Map<ProductIdentifierDto>(productFromRepo);

            return CreateLinks(mappedProduct);
        }

        private ProductIdentifierDto CreateLinks(ProductIdentifierDto mappedProduct)
        {
            mappedProduct.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Product", mappedProduct.Id), "self", "GET"));

            return mappedProduct;
        }
    }
}
