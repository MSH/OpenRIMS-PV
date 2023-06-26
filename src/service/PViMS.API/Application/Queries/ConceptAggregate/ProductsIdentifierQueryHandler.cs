using AutoMapper;
using LinqKit;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Helpers;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.API.Models.ValueTypes;
using PVIMS.Core.Aggregates.ConceptAggregate;
using PVIMS.Core.Paging;
using PVIMS.Core.Repositories;
using Extensions = PVIMS.Core.Utilities.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.ConceptAggregate
{
    public class ProductsIdentifierQueryHandler
        : IRequestHandler<ProductsIdentifierQuery, LinkedCollectionResourceWrapperDto<ProductIdentifierDto>>
    {
        private readonly IRepositoryInt<Product> _productRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductsIdentifierQueryHandler> _logger;

        public ProductsIdentifierQueryHandler(
            IRepositoryInt<Product> productRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<ProductsIdentifierQueryHandler> logger)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<ProductIdentifierDto>> Handle(ProductsIdentifierQuery message, CancellationToken cancellationToken)
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = message.PageNumber,
                PageSize = message.PageSize
            };

            var orderby = Extensions.GetOrderBy<Product>(message.OrderBy, "asc");

            var predicate = PredicateBuilder.New<Product>(true);
            if (message.Active != YesNoBothValueType.Both)
            {
                predicate = predicate.And(c => c.Active == (message.Active == YesNoBothValueType.Yes));
            }
            if (!String.IsNullOrWhiteSpace(message.SearchTerm))
            {
                predicate = predicate.And(c => c.ProductName.Contains(message.SearchTerm.Trim()));
            }

            var pagedProductsFromRepo = await _productRepository.ListAsync(pagingInfo, predicate, orderby, new string[] { "Concept.MedicationForm" });
            if (pagedProductsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedProducts = PagedCollection<ProductIdentifierDto>.Create(_mapper.Map<PagedCollection<ProductIdentifierDto>>(pagedProductsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedProductsFromRepo.TotalCount);

                // Add HATEOAS links to each individual resource
                mappedProducts.ForEach(dto => CreateLinks(dto));

                var wrapper = new LinkedCollectionResourceWrapperDto<ProductIdentifierDto>(pagedProductsFromRepo.TotalCount, mappedProducts, pagedProductsFromRepo.TotalPages);

                CreateLinksForProducts(wrapper, message.OrderBy, message.SearchTerm, message.Active, message.PageNumber, message.PageSize,
                    pagedProductsFromRepo.HasNext, pagedProductsFromRepo.HasPrevious);

                return wrapper;

            }

            return null;
        }

        private void CreateLinks(ProductIdentifierDto mappedProduct)
        {
            mappedProduct.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Product", mappedProduct.Id), "self", "GET"));
        }

        private void CreateLinksForProducts(
            LinkedResourceBaseDto wrapper,
            string orderBy,
            string searchTerm,
            YesNoBothValueType active,
            int pageNumber, int pageSize,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateProductsResourceUri(ResourceUriType.Current, orderBy, searchTerm, active, pageNumber, pageSize),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateProductsResourceUri(ResourceUriType.NextPage, orderBy, searchTerm, active, pageNumber, pageSize),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateProductsResourceUri(ResourceUriType.PreviousPage, orderBy, searchTerm, active, pageNumber, pageSize),
                       "previousPage", "GET"));
            }
        }
    }
}
