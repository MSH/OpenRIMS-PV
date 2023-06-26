using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Aggregates.ConceptAggregate;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.ConceptAggregate
{
    public class ProductDetailQueryHandler
        : IRequestHandler<ProductDetailQuery, ProductDetailDto>
    {
        private readonly IRepositoryInt<Product> _productRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductDetailQueryHandler> _logger;

        public ProductDetailQueryHandler(
            IRepositoryInt<Product> productRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<ProductDetailQueryHandler> logger)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ProductDetailDto> Handle(ProductDetailQuery message, CancellationToken cancellationToken)
        {
            var productFromRepo = await _productRepository.GetAsync(p => p.Id == message.ProductId, new string[] { "Concept.MedicationForm" });

            if (productFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate product {message.ProductId}");
            }

            var mappedProduct = _mapper.Map<ProductDetailDto>(productFromRepo);

            CreateLinks(mappedProduct);

            return mappedProduct;
        }

        private void CreateLinks(ProductDetailDto mappedProduct)
        {
            mappedProduct.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Product", mappedProduct.Id), "self", "GET"));
        }
    }
}
