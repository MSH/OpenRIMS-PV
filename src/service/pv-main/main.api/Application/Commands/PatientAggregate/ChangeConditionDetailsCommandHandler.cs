using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Exceptions;
using OpenRIMS.PV.Main.Core.Models;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.PatientAggregate
{
    public class ChangeConditionDetailsCommandHandler
        : IRequestHandler<ChangeConditionDetailsCommand, bool>
    {
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<Outcome> _outcomeRepository;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<PatientStatus> _patientStatusRepository;
        private readonly IRepositoryInt<TerminologyMedDra> _terminologyMeddraRepository;
        private readonly IRepositoryInt<TreatmentOutcome> _treatmentOutcomeRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ChangeConditionDetailsCommandHandler> _logger;

        public ChangeConditionDetailsCommandHandler(
            ITypeExtensionHandler modelExtensionBuilder,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<Outcome> outcomeRepository,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<PatientStatus> patientStatusRepository,
            IRepositoryInt<TerminologyMedDra> terminologyMeddraRepository,
            IRepositoryInt<TreatmentOutcome> treatmentOutcomeRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<ChangeConditionDetailsCommandHandler> logger)
        {
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _outcomeRepository = outcomeRepository ?? throw new ArgumentNullException(nameof(outcomeRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _patientStatusRepository = patientStatusRepository ?? throw new ArgumentNullException(nameof(patientStatusRepository));
            _terminologyMeddraRepository = terminologyMeddraRepository ?? throw new ArgumentNullException(nameof(terminologyMeddraRepository));
            _treatmentOutcomeRepository = treatmentOutcomeRepository ?? throw new ArgumentNullException(nameof(treatmentOutcomeRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangeConditionDetailsCommand message, CancellationToken cancellationToken)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == message.PatientId, new string[] { "PatientConditions.TerminologyMedDra", "PatientStatusHistories.PatientStatus" });
            if (patientFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate patient");
            }

            var sourceTerminologyFromRepo = _terminologyMeddraRepository.Get(message.SourceTerminologyMedDraId);
            if (sourceTerminologyFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate source terminology");
            }

            Outcome outcomeFromRepo = null;
            if (!String.IsNullOrWhiteSpace(message.Outcome))
            {
                outcomeFromRepo = _outcomeRepository.Get(o => o.Description == message.Outcome);
                if (outcomeFromRepo == null)
                {
                    throw new KeyNotFoundException("Unable to locate outcome");
                }
            }

            TreatmentOutcome treatmentOutcomeFromRepo = null;
            if (!String.IsNullOrWhiteSpace(message.TreatmentOutcome))
            {
                treatmentOutcomeFromRepo = _treatmentOutcomeRepository.Get(to => to.Description == message.TreatmentOutcome);
                if (treatmentOutcomeFromRepo == null)
                {
                    throw new KeyNotFoundException("Unable to locate treatment outcome");
                }
            }

            if (outcomeFromRepo != null && treatmentOutcomeFromRepo != null)
            {
                if (outcomeFromRepo.Description == "Fatal" && treatmentOutcomeFromRepo.Description != "Died")
                {
                    throw new DomainException("Treatment Outcome not consistent with Condition Outcome");
                }
                if (outcomeFromRepo.Description != "Fatal" && treatmentOutcomeFromRepo.Description == "Died")
                {
                    throw new DomainException("Condition Outcome not consistent with Treatment Outcome");
                }
            }

            var conditionDetail = await PrepareConditionDetailAsync(message.Attributes);
            if (!conditionDetail.IsValid())
            {
                conditionDetail.InvalidAttributes.ForEach(element => throw new DomainException(element));
            }

            patientFromRepo.ChangeConditionDetails(message.PatientConditionId, message.SourceTerminologyMedDraId, message.StartDate, message.OutcomeDate, outcomeFromRepo, treatmentOutcomeFromRepo, message.CaseNumber, message.Comments);
            _modelExtensionBuilder.UpdateExtendable(patientFromRepo.PatientConditions.Single(pm => pm.Id == message.PatientConditionId), conditionDetail.CustomAttributes, "Admin");

            if (outcomeFromRepo?.Description == "Fatal" && patientFromRepo.GetCurrentStatus().PatientStatus.Description != "Died")
            {
                var patientStatus = await _patientStatusRepository.GetAsync(ps => ps.Description == "Died");
                patientFromRepo.ChangePatientStatus(patientStatus, message.OutcomeDate ?? DateTime.Now, $"Marked as deceased through condition ({sourceTerminologyFromRepo.DisplayName})");
            }

            _patientRepository.Update(patientFromRepo);

            _logger.LogInformation($"----- Condition {message.PatientConditionId} details updated");

            return await _unitOfWork.CompleteAsync();
        }

        private async Task<ConditionDetail> PrepareConditionDetailAsync(IDictionary<int, string> attributes)
        {
            var conditionDetail = new ConditionDetail();
            conditionDetail.CustomAttributes = _modelExtensionBuilder.BuildModelExtension<PatientCondition>();

            //conditionDetail = _mapper.Map<ConditionDetail>(conditionForUpdate);
            foreach (var newAttribute in attributes)
            {
                var customAttribute = await _customAttributeRepository.GetAsync(ca => ca.Id == newAttribute.Key);
                if (customAttribute == null)
                {
                    throw new KeyNotFoundException($"Unable to locate custom attribute {newAttribute.Key}");
                }

                var attributeDetail = conditionDetail.CustomAttributes.SingleOrDefault(ca => ca.AttributeKey == customAttribute.AttributeKey);

                if (attributeDetail == null)
                {
                    throw new KeyNotFoundException($"Unable to locate custom attribute on patient condition {newAttribute.Key}");
                }

                attributeDetail.Value = newAttribute.Value.Trim();
            }

            return conditionDetail;
        }
    }
}
