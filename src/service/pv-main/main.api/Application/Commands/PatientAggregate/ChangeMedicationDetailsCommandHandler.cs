using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.Core.CustomAttributes;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Models;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.Services;
using OpenRIMS.PV.Main.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.PatientAggregate
{
    public class ChangeMedicationDetailsCommandHandler
        : IRequestHandler<ChangeMedicationDetailsCommand, bool>
    {
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly IRepositoryInt<Config> _configRepository;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IWorkFlowService _workFlowService;
        private readonly ILogger<ChangeMedicationDetailsCommandHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChangeMedicationDetailsCommandHandler(
            ITypeExtensionHandler modelExtensionBuilder,
            IRepositoryInt<Config> configRepository,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<Patient> patientRepository,
            IUnitOfWorkInt unitOfWork,
            IWorkFlowService workFlowService,
            ILogger<ChangeMedicationDetailsCommandHandler> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _workFlowService = workFlowService ?? throw new ArgumentNullException(nameof(workFlowService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<bool> Handle(ChangeMedicationDetailsCommand message, CancellationToken cancellationToken)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == message.PatientId, new string[] { "PatientClinicalEvents", "PatientMedications.Concept.MedicationForm", "PatientMedications.Product" });
            if (patientFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate patient");
            }

            var medicationToUpdate = patientFromRepo.PatientMedications.Single(pm => pm.Id == message.PatientMedicationId);
            var medicationAttributes = await PrepareMedicationAttributesWithNewValuesAsync(medicationToUpdate, message.Attributes);
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            patientFromRepo.ChangeMedicationDetails(message.PatientMedicationId, message.StartDate, message.EndDate, message.Dose, message.DoseFrequency, message.DoseUnit);

            _modelExtensionBuilder.ValidateAndUpdateExtendable(medicationToUpdate, medicationAttributes, userName);

            _patientRepository.Update(patientFromRepo);

            await RefreshMedicationOnMatchingReportInstancesAsync(patientFromRepo, message.StartDate, message.EndDate, patientFromRepo.PatientMedications.Single(pm => pm.Id == message.PatientMedicationId).DisplayName, patientFromRepo.PatientMedications.Single(pm => pm.Id == message.PatientMedicationId).PatientMedicationGuid);

            _logger.LogInformation($"----- Medication {message.PatientMedicationId} details updated");

            return await _unitOfWork.CompleteAsync();
        }

        private async Task<List<CustomAttributeDetail>> PrepareMedicationAttributesWithNewValuesAsync(IExtendable extendedEntity, IDictionary<int, string> newAttributes)
        {
            var currentAttributes = _modelExtensionBuilder.BuildModelExtension(extendedEntity);

            await PopulateAttributesWithNewValues(newAttributes, currentAttributes);

            return currentAttributes;
        }

        private async Task PopulateAttributesWithNewValues(IDictionary<int, string> attributes, List<CustomAttributeDetail> customAttributes)
        {
            foreach (var newAttribute in attributes)
            {
                var customAttribute = await _customAttributeRepository.GetAsync(ca => ca.Id == newAttribute.Key);
                if (customAttribute == null)
                {
                    throw new KeyNotFoundException($"Unable to locate custom attribute {newAttribute.Key}");
                }

                var attributeDetail = customAttributes.SingleOrDefault(ca => ca.AttributeKey == customAttribute.AttributeKey);
                if (attributeDetail == null)
                {
                    throw new KeyNotFoundException($"Unable to locate custom attribute on patient {newAttribute.Key}");
                }

                attributeDetail.Value = newAttribute.Value;
            }
        }

        private async Task RefreshMedicationOnMatchingReportInstancesAsync(Patient patientFromRepo, DateTime medicationStartDate, DateTime? medicationEndDate, string medicationDisplayName, Guid medicationGuid)
        {
            var weeks = await GetNumberOfWeeksToCheckAsync();

            var clinicalEventsToBeLinked = GetClinicalEventsWhichMatchMedicationPeriod(patientFromRepo, medicationStartDate, medicationEndDate, weeks);
            var medicationToBeLinked = PrepareMedicationForLinkingToReportInstances(medicationDisplayName, medicationGuid);

            await LinkMedicationToReportInstances(clinicalEventsToBeLinked, medicationToBeLinked);
        }

        private async Task<int> GetNumberOfWeeksToCheckAsync()
        {
            var config = await _configRepository.GetAsync(c => c.ConfigType == ConfigType.MedicationOnsetCheckPeriodWeeks);
            if (config != null)
            {
                if (!String.IsNullOrEmpty(config.ConfigValue))
                {
                    return Convert.ToInt32(config.ConfigValue);
                }
            }
            return 0;
        }

        private IEnumerable<PatientClinicalEvent> GetClinicalEventsWhichMatchMedicationPeriod(Patient patientFromRepo, DateTime medicationStartDate, DateTime? medicationEndDate, int weekCount)
        {
            if (!medicationEndDate.HasValue)
            {
                return patientFromRepo.PatientClinicalEvents.Where(pce => pce.OnsetDate >= medicationStartDate.AddDays(weekCount * -7) && pce.Archived == false);
            }

            return patientFromRepo.PatientClinicalEvents.Where(pce => pce.OnsetDate >= medicationStartDate.AddDays(weekCount * -7) && pce.OnsetDate <= medicationEndDate.Value.AddDays(weekCount * 7) && pce.Archived == false);
        }

        private List<ReportInstanceMedicationListItem> PrepareMedicationForLinkingToReportInstances(string displayName, Guid patientMedicationGuid)
        {
            var instanceMedications = new List<ReportInstanceMedicationListItem>();
            instanceMedications.Add(new ReportInstanceMedicationListItem()
            {
                MedicationIdentifier = displayName,
                ReportInstanceMedicationGuid = patientMedicationGuid
            });
            return instanceMedications;
        }

        private async Task LinkMedicationToReportInstances(IEnumerable<PatientClinicalEvent> clinicalEvents, List<ReportInstanceMedicationListItem> medications)
        {
            foreach (var clinicalEvent in clinicalEvents)
            {
                await _workFlowService.AddOrUpdateMedicationsForWorkFlowInstanceAsync(clinicalEvent.PatientClinicalEventGuid, medications);
            }
        }
    }
}
