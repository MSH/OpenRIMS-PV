using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Helpers;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Paging;
using OpenRIMS.PV.Main.Core.Repositories;
using Extensions = OpenRIMS.PV.Main.Core.Utilities.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;
using OpenRIMS.PV.Main.Core.Aggregates.MetaFormAggregate;

namespace OpenRIMS.PV.Main.API.Application.Queries.MetaFormAggregate
{
    public class MetaFormsDetailQueryHandler
        : IRequestHandler<MetaFormsDetailQuery, LinkedCollectionResourceWrapperDto<MetaFormDetailDto>>
    {
        private readonly IRepositoryInt<MetaForm> _metaFormRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<MetaFormsDetailQueryHandler> _logger;

        public MetaFormsDetailQueryHandler(
            IRepositoryInt<MetaForm> metaFormRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<MetaFormsDetailQueryHandler> logger)
        {
            _metaFormRepository = metaFormRepository ?? throw new ArgumentNullException(nameof(metaFormRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<MetaFormDetailDto>> Handle(MetaFormsDetailQuery message, CancellationToken cancellationToken)
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = message.PageNumber,
                PageSize = message.PageSize
            };

            var orderby = Extensions.GetOrderBy<MetaForm>(message.OrderBy, "asc");

            var pagedMetaFormsFromRepo = await _metaFormRepository.ListAsync(pagingInfo, null, orderby, new string[] { "CohortGroup" });
            if (pagedMetaFormsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedMetaForms = PagedCollection<MetaFormDetailDto>.Create(_mapper.Map<PagedCollection<MetaFormDetailDto>>(pagedMetaFormsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedMetaFormsFromRepo.TotalCount);

                // Add HATEOAS links to each individual resource
                mappedMetaForms.ForEach(dto => CreateLinks(dto));

                var wrapper = new LinkedCollectionResourceWrapperDto<MetaFormDetailDto>(pagedMetaFormsFromRepo.TotalCount, mappedMetaForms, pagedMetaFormsFromRepo.TotalPages);

                CreateLinksForMetaForms(wrapper, message.OrderBy, message.PageNumber, message.PageSize,
                    pagedMetaFormsFromRepo.HasNext, pagedMetaFormsFromRepo.HasPrevious);

                return wrapper;

            }

            return null;
        }

        private void CreateLinks(MetaFormDetailDto mappedMetaForm)
        {
            mappedMetaForm.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("MetaForm", mappedMetaForm.Id), "self", "GET"));
        }

        private void CreateLinksForMetaForms(
            LinkedResourceBaseDto wrapper,
            string orderBy,
            int pageNumber, int pageSize,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.Current, "GetMetaFormsByDetail", orderBy, pageNumber, pageSize),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.NextPage, "GetMetaFormsByDetail", orderBy, pageNumber, pageSize),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.PreviousPage, "GetMetaFormsByDetail", orderBy, pageNumber, pageSize),
                       "previousPage", "GET"));
            }
        }
    }
}
