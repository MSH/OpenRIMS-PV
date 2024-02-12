using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.Core.CustomAttributes;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.PatientAggregate
{
    public class ChangeClinicalEventDetailsCommandHandler
        : IRequestHandler<ChangeClinicalEventDetailsCommand, bool>
    {
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<TerminologyMedDra> _terminologyMeddraRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IWorkFlowService _workFlowService;
        private readonly ILogger<ChangeMedicationDetailsCommandHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChangeClinicalEventDetailsCommandHandler(
            ITypeExtensionHandler modelExtensionBuilder,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<TerminologyMedDra> terminologyMeddraRepository,
            IUnitOfWorkInt unitOfWork,
            IWorkFlowService workFlowService,
            ILogger<ChangeMedicationDetailsCommandHandler> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _terminologyMeddraRepository = terminologyMeddraRepository ?? throw new ArgumentNullException(nameof(terminologyMeddraRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _workFlowService = workFlowService ?? throw new ArgumentNullException(nameof(workFlowService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<bool> Handle(ChangeClinicalEventDetailsCommand message, CancellationToken cancellationToken)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == message.PatientId, new string[] { 
                "PatientClinicalEvents.SourceTerminologyMedDra",
                "PatientConditions.TerminologyMedDra.ConditionMedDras.Condition"
            });
            if (patientFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate patient");
            }

            TerminologyMedDra sourceTermFromRepo = null;
            if (message.SourceTerminologyMedDraId.HasValue)
            {
                sourceTermFromRepo = await _terminologyMeddraRepository.GetAsync(message.SourceTerminologyMedDraId); ;
                if (sourceTermFromRepo == null)
                {
                    throw new KeyNotFoundException("Unable to locate terminology for MedDRA");
                }
            }

            var clinicalEventToUpdate = patientFromRepo.PatientClinicalEvents.Single(pce => pce.Id == message.PatientClinicalEventId);
            var clinicalEventAttributes = await PrepareClinicalEventAttributesWithNewValuesAsync(clinicalEventToUpdate, message.Attributes);
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            patientFromRepo.ChangeClinicalEventDetails(message.PatientClinicalEventId, message.OnsetDate, message.ResolutionDate, sourceTermFromRepo, message.SourceDescription);

            _modelExtensionBuilder.ValidateAndUpdateExtendable(clinicalEventToUpdate, clinicalEventAttributes, userName);

            _patientRepository.Update(patientFromRepo);

            // TODO Move to domain event
            await UpdateReportInstanceIdentifiers(patientFromRepo, clinicalEventToUpdate);

            _logger.LogInformation($"----- Clinical Event {message.PatientClinicalEventId} details updated");

            return await _unitOfWork.CompleteAsync();
        }

        private async Task UpdateReportInstanceIdentifiers(Patient patientFromRepo, PatientClinicalEvent clinicalEventToUpdate)
        {
            await _workFlowService.UpdateSourceIdentifierForReportInstanceAsync(
                contextGuid: clinicalEventToUpdate.PatientClinicalEventGuid,
                sourceIdentifier: clinicalEventToUpdate.SourceTerminologyMedDra?.DisplayName ?? clinicalEventToUpdate.SourceDescription);
        }

        private async Task<List<CustomAttributeDetail>> PrepareClinicalEventAttributesWithNewValuesAsync(IExtendable extendedEntity, IDictionary<int, string> newAttributes)
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
    }
}