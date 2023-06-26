using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Commands.PatientAggregate
{
    public class ChangePatientNameCommandHandler
        : IRequestHandler<ChangePatientNameCommand, bool>
    {
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IWorkFlowService _workFlowService;
        private readonly ILogger<ChangePatientNameCommandHandler> _logger;

        public ChangePatientNameCommandHandler(
            IRepositoryInt<Patient> patientRepository,
            IUnitOfWorkInt unitOfWork,
            IWorkFlowService workFlowService,
            ILogger<ChangePatientNameCommandHandler> logger)
        {
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _workFlowService = workFlowService ?? throw new ArgumentNullException(nameof(workFlowService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangePatientNameCommand message, CancellationToken cancellationToken)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == message.PatientId, new string[] {
                "PatientClinicalEvents.SourceTerminologyMedDra",
                "PatientConditions.TerminologyMedDra.ConditionMedDras.Condition"
            });

            patientFromRepo.ChangePatientName(message.FirstName, message.MiddleName, message.LastName);
            _patientRepository.Update(patientFromRepo);

            // TODO Move to domain event
            await UpdateReportInstanceIdentifiers(patientFromRepo);

            _logger.LogInformation($"----- Patient {message.PatientId} name details updated");

            return await _unitOfWork.CompleteAsync();
        }

        private async Task UpdateReportInstanceIdentifiers(Patient patientFromRepo)
        {
            var primaryCondition = patientFromRepo.GetConditionForGroupAndDate("TB", DateTime.Today);

            foreach (var patientClinicalEvent in patientFromRepo.PatientClinicalEvents)
            {
                await _workFlowService.UpdatePatientIdentifierForReportInstanceAsync(
                    contextGuid: patientClinicalEvent.PatientClinicalEventGuid,
                    patientIdentifier: primaryCondition != null ? $"{patientFromRepo.FullName} ({primaryCondition.CaseNumber.Trim()})" : patientFromRepo.FullName);
            }
        }
    }
}
