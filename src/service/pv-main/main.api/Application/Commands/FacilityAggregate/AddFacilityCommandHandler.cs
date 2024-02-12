using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Exceptions;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.FacilityAggregate
{
    public class AddFacilityCommandHandler
        : IRequestHandler<AddFacilityCommand, FacilityDetailDto>
    {
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly IRepositoryInt<FacilityType> _facilityTypeRepository;
        private readonly IRepositoryInt<OrgUnit> _orgUnitRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<AddFacilityCommandHandler> _logger;

        public AddFacilityCommandHandler(
            IRepositoryInt<Facility> facilityRepository,
            IRepositoryInt<FacilityType> facilityTypeRepository,
            IRepositoryInt<OrgUnit> orgUnitRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<AddFacilityCommandHandler> logger)
        {
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(facilityRepository));
            _facilityTypeRepository = facilityTypeRepository ?? throw new ArgumentNullException(nameof(facilityTypeRepository));
            _orgUnitRepository = orgUnitRepository ?? throw new ArgumentNullException(nameof(orgUnitRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<FacilityDetailDto> Handle(AddFacilityCommand message, CancellationToken cancellationToken)
        {
            var facilityTypeFromRepo = await _facilityTypeRepository.GetAsync(c => c.Description == message.FacilityType);
            if (facilityTypeFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate facility type");
            }

            if (_facilityRepository.Exists(f => f.FacilityName == message.FacilityName ||
                f.FacilityCode == message.FacilityCode))
            {
                throw new DomainException("Facility with same name or code already exists");
            }

            OrgUnit orgUnitFromRepo = null;
            if(message.OrgUnitId.HasValue)
            {
                if(message.OrgUnitId > 0 )
                {
                    orgUnitFromRepo = await _orgUnitRepository.GetAsync(message.OrgUnitId);
                    if (orgUnitFromRepo == null)
                    {
                        throw new KeyNotFoundException($"Unable to locate organisation unit {message.OrgUnitId}");
                    }
                }
            }

            var newFacility = new Facility(message.FacilityName, message.FacilityCode, facilityTypeFromRepo, message.TelNumber, message.MobileNumber, message.FaxNumber, orgUnitFromRepo);

            await _facilityRepository.SaveAsync(newFacility);

            _logger.LogInformation($"----- Facility {message.FacilityName} created");

            var mappedFacility = _mapper.Map<FacilityDetailDto>(newFacility);

            return CreateLinks(mappedFacility);
        }

        private FacilityDetailDto CreateLinks(FacilityDetailDto dto)
        {
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Facility", dto.Id), "self", "GET"));

            return dto;
        }
    }
}
