using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.Core.Entities;
using PVIMS.Core.Exceptions;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Commands.FacilityAggregate
{
    public class DeleteFacilityCommandHandler
        : IRequestHandler<DeleteFacilityCommand, bool>
    {
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<DeleteFacilityCommandHandler> _logger;

        public DeleteFacilityCommandHandler(
            IRepositoryInt<Facility> facilityRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<DeleteFacilityCommandHandler> logger)
        {
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(facilityRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(DeleteFacilityCommand message, CancellationToken cancellationToken)
        {
            var facilityFromRepo = await _facilityRepository.GetAsync(f => f.Id == message.Id, new string[] { "PatientFacilities", "UserFacilities" });
            if (facilityFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate facility");
            }

            if (facilityFromRepo.PatientFacilities.Count > 0 || facilityFromRepo.UserFacilities.Count > 0)
            {
                throw new DomainException("Unable to delete the Facility as it is currently in use");
            }

            _facilityRepository.Delete(facilityFromRepo);

            _logger.LogInformation($"----- Facility {message.Id} deleted");

            return await _unitOfWork.CompleteAsync();
        }
    }
}
