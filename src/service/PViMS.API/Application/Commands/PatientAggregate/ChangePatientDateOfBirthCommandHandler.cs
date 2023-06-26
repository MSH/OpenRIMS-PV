using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Commands.PatientAggregate
{
    public class ChangePatientDateOfBirthCommandHandler
        : IRequestHandler<ChangePatientDateOfBirthCommand, bool>
    {
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ChangePatientDateOfBirthCommandHandler> _logger;

        public ChangePatientDateOfBirthCommandHandler(
            IRepositoryInt<Patient> patientRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<ChangePatientDateOfBirthCommandHandler> logger)
        {
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangePatientDateOfBirthCommand message, CancellationToken cancellationToken)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == message.PatientId);
            if (patientFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate patient");
            }

            patientFromRepo.ChangePatientDateOfBirth(message.DateOfBirth);
            _patientRepository.Update(patientFromRepo);

            _logger.LogInformation($"----- Patient {message.PatientId} date of birth details updated");

            return await _unitOfWork.CompleteAsync();
        }
    }
}
