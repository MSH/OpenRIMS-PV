using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Exceptions;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.FacilityAggregate
{
    public class ChangeFacilityDetailsCommandHandler
        : IRequestHandler<ChangeFacilityDetailsCommand, bool>
    {
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly IRepositoryInt<FacilityType> _facilityTypeRepository;
        private readonly IRepositoryInt<OrgUnit> _orgUnitRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ChangeFacilityDetailsCommandHandler> _logger;

        public ChangeFacilityDetailsCommandHandler(
            IRepositoryInt<Facility> facilityRepository,
            IRepositoryInt<FacilityType> facilityTypeRepository,
            IRepositoryInt<OrgUnit> orgUnitRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<ChangeFacilityDetailsCommandHandler> logger)
        {
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(facilityRepository));
            _facilityTypeRepository = facilityTypeRepository ?? throw new ArgumentNullException(nameof(facilityTypeRepository));
            _orgUnitRepository = orgUnitRepository ?? throw new ArgumentNullException(nameof(orgUnitRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangeFacilityDetailsCommand message, CancellationToken cancellationToken)
        {
            var facilityFromRepo = await _facilityRepository.GetAsync(f => f.Id == message.Id);
            if (facilityFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate facility");
            }

            var facilityTypeFromRepo = await _facilityTypeRepository.GetAsync(c => c.Description == message.FacilityType);
            if (facilityTypeFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate facility type");
            }

            OrgUnit orgUnitFromRepo = null;
            if (message.OrgUnitId.HasValue)
            {
                if (message.OrgUnitId > 0)
                {
                    orgUnitFromRepo = await _orgUnitRepository.GetAsync(message.OrgUnitId);
                    if (orgUnitFromRepo == null)
                    {
                        throw new KeyNotFoundException($"Unable to locate organisation unit {message.OrgUnitId}");
                    }
                }
            }

            if (_facilityRepository.Exists(l => (l.FacilityName == message.FacilityName || l.FacilityCode == message.FacilityCode) && l.Id != message.Id))
            {
                throw new DomainException("Item with same name already exists");
            }

            facilityFromRepo.ChangeDetails(message.FacilityName, message.FacilityCode, facilityTypeFromRepo, message.TelNumber, message.MobileNumber, message.FaxNumber, orgUnitFromRepo);
            _facilityRepository.Update(facilityFromRepo);

            _logger.LogInformation($"----- Facility {message.FacilityName} details updated");

            return await _unitOfWork.CompleteAsync();
        }
    }
}
