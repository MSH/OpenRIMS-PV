using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Queries.FacilityAggregate
{
    public class FacilityDetailQueryHandler
        : IRequestHandler<FacilityDetailQuery, FacilityDetailDto>
    {
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<FacilityDetailQueryHandler> _logger;

        public FacilityDetailQueryHandler(
            IRepositoryInt<Facility> facilityRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<FacilityDetailQueryHandler> logger)
        {
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(facilityRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<FacilityDetailDto> Handle(FacilityDetailQuery message, CancellationToken cancellationToken)
        {
            var facilityFromRepo = await _facilityRepository.GetAsync(f => f.Id == message.FacilityId, new string[] { "FacilityType" });

            if (facilityFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate facility");
            }

            var mappedFacility = _mapper.Map<FacilityDetailDto>(facilityFromRepo);

            return CreateLinks(mappedFacility);
        }

        private FacilityDetailDto CreateLinks(FacilityDetailDto mappedFacility)
        {
            mappedFacility.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Facility", mappedFacility.Id), "self", "GET"));
            mappedFacility.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Facility", mappedFacility.Id), "self", "DELETE"));

            return mappedFacility;
        }
    }
}
