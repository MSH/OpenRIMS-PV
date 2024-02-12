using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Helpers;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Paging;
using OpenRIMS.PV.Main.Core.Repositories;
using Extensions = OpenRIMS.PV.Main.Core.Utilities.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Queries.ConceptAggregate
{
    public class MedicationFormsIdentifierQueryHandler
        : IRequestHandler<MedicationFormsIdentifierQuery, LinkedCollectionResourceWrapperDto<MedicationFormIdentifierDto>>
    {
        private readonly IRepositoryInt<MedicationForm> _medicationFormRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<MedicationFormsIdentifierQueryHandler> _logger;

        public MedicationFormsIdentifierQueryHandler(
            IRepositoryInt<MedicationForm> medicationFormRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<MedicationFormsIdentifierQueryHandler> logger)
        {
            _medicationFormRepository = medicationFormRepository ?? throw new ArgumentNullException(nameof(medicationFormRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<MedicationFormIdentifierDto>> Handle(MedicationFormsIdentifierQuery message, CancellationToken cancellationToken)
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = message.PageNumber,
                PageSize = message.PageSize
            };

            var orderby = Extensions.GetOrderBy<MedicationForm>(message.OrderBy, "asc");

            var pagedMedicationFormsFromRepo = await _medicationFormRepository.ListAsync(pagingInfo, null, orderby, new string[] { "" });
            if (pagedMedicationFormsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedMedicationForms = PagedCollection<MedicationFormIdentifierDto>.Create(_mapper.Map<PagedCollection<MedicationFormIdentifierDto>>(pagedMedicationFormsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedMedicationFormsFromRepo.TotalCount);

                // Add HATEOAS links to each individual resource
                mappedMedicationForms.ForEach(dto => CreateLinks(dto));

                var wrapper = new LinkedCollectionResourceWrapperDto<MedicationFormIdentifierDto>(pagedMedicationFormsFromRepo.TotalCount, mappedMedicationForms, pagedMedicationFormsFromRepo.TotalPages);

                CreateLinksForMedicationForms(wrapper, message.OrderBy,message.PageNumber, message.PageSize,
                    pagedMedicationFormsFromRepo.HasNext, pagedMedicationFormsFromRepo.HasPrevious);

                return wrapper;

            }

            return null;
        }

        private void CreateLinks(MedicationFormIdentifierDto mappedConcept)
        {
            mappedConcept.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("MedicationForm", mappedConcept.Id), "self", "GET"));
        }

        private void CreateLinksForMedicationForms(
            LinkedResourceBaseDto wrapper,
            string orderBy,
            int pageNumber, int pageSize,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateMedicationFormsResourceUri(ResourceUriType.Current, orderBy, pageNumber, pageSize),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateMedicationFormsResourceUri(ResourceUriType.NextPage, orderBy, pageNumber, pageSize),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateMedicationFormsResourceUri(ResourceUriType.PreviousPage, orderBy, pageNumber, pageSize),
                       "previousPage", "GET"));
            }
        }
    }
}
