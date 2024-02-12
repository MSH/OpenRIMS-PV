using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.PatientAggregate
{
    public class ChangePatientFacilityCommandHandler
        : IRequestHandler<ChangePatientFacilityCommand, bool>
    {
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ChangePatientFacilityCommandHandler> _logger;

        public ChangePatientFacilityCommandHandler(
            IRepositoryInt<Facility> facilityRepository,
            IRepositoryInt<Patient> patientRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<ChangePatientFacilityCommandHandler> logger)
        {
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(facilityRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangePatientFacilityCommand message, CancellationToken cancellationToken)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == message.PatientId);
            if (patientFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate patient");
            }

            var facilityFromRepo = _facilityRepository.Get(f => f.FacilityName == message.FacilityName);
            if (facilityFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate facility");
            }

            patientFromRepo.ChangePatientFacility(facilityFromRepo);
            _patientRepository.Update(patientFromRepo);

            _logger.LogInformation($"----- Patient {message.PatientId} facility details updated");

            return await _unitOfWork.CompleteAsync();
        }
    }
}
