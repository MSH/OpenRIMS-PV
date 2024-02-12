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
    public class ChangePatientNotesCommandHandler
        : IRequestHandler<ChangePatientNotesCommand, bool>
    {
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ChangePatientNotesCommandHandler> _logger;

        public ChangePatientNotesCommandHandler(
            IRepositoryInt<Patient> patientRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<ChangePatientNotesCommandHandler> logger)
        {
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangePatientNotesCommand message, CancellationToken cancellationToken)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == message.PatientId);
            if (patientFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate patient");
            }

            patientFromRepo.ChangePatientNotes(message.Notes);
            _patientRepository.Update(patientFromRepo);

            _logger.LogInformation($"----- Patient {message.PatientId} notes details updated");

            return await _unitOfWork.CompleteAsync();
        }
    }
}
