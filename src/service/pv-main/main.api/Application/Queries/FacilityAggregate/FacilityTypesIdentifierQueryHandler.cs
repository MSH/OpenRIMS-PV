using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Paging;
using OpenRIMS.PV.Main.Core.Repositories;
using Extensions = OpenRIMS.PV.Main.Core.Utilities.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Queries.FacilityAggregate
{
    public class FacilityTypesIdentifierQueryHandler
        : IRequestHandler<FacilityTypesIdentifierQuery, LinkedCollectionResourceWrapperDto<FacilityTypeIdentifierDto>>
    {
        private readonly IRepositoryInt<FacilityType> _facilityTypeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<FacilityTypesIdentifierQueryHandler> _logger;

        public FacilityTypesIdentifierQueryHandler(
            IRepositoryInt<FacilityType> facilityTypeRepository,
            IMapper mapper,
            ILogger<FacilityTypesIdentifierQueryHandler> logger)
        {
            _facilityTypeRepository = facilityTypeRepository ?? throw new ArgumentNullException(nameof(facilityTypeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<FacilityTypeIdentifierDto>> Handle(FacilityTypesIdentifierQuery message, CancellationToken cancellationToken)
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = message.PageNumber,
                PageSize = message.PageSize
            };

            var orderby = Extensions.GetOrderBy<FacilityType>(message.OrderBy, "asc");

            var pagedFacilityTypesFromRepo = await _facilityTypeRepository.ListAsync(pagingInfo, null, orderby, "");
            if (pagedFacilityTypesFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedFacilityTypes = PagedCollection<FacilityTypeIdentifierDto>.Create(_mapper.Map<PagedCollection<FacilityTypeIdentifierDto>>(pagedFacilityTypesFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedFacilityTypesFromRepo.TotalCount);

                var wrapper = new LinkedCollectionResourceWrapperDto<FacilityTypeIdentifierDto>(pagedFacilityTypesFromRepo.TotalCount, mappedFacilityTypes, pagedFacilityTypesFromRepo.TotalPages);

                return wrapper;
            }

            return null;
        }
    }
}
