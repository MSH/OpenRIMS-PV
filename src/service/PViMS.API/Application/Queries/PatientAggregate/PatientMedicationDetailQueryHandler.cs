using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.PatientAggregate
{
    public class PatientMedicationDetailQueryHandler
        : IRequestHandler<PatientMedicationDetailQuery, PatientMedicationDetailDto>
    {
        private readonly IRepositoryInt<PatientMedication> _patientMedicationRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly ICustomAttributeService _customAttributeService;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientMedicationDetailQueryHandler> _logger;

        public PatientMedicationDetailQueryHandler(
            IRepositoryInt<PatientMedication> patientMedicationRepository,
            IRepositoryInt<SelectionDataItem> selectionDataItemRepository,
            ITypeExtensionHandler modelExtensionBuilder,
            ILinkGeneratorService linkGeneratorService,
            ICustomAttributeService customAttributeService,
            IMapper mapper,
            ILogger<PatientMedicationDetailQueryHandler> logger)
        {
            _patientMedicationRepository = patientMedicationRepository ?? throw new ArgumentNullException(nameof(patientMedicationRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _customAttributeService = customAttributeService ?? throw new ArgumentNullException(nameof(customAttributeService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PatientMedicationDetailDto> Handle(PatientMedicationDetailQuery message, CancellationToken cancellationToken)
        {
            var patientMedicationFromRepo = await _patientMedicationRepository.GetAsync(pm => pm.Patient.Id == message.PatientId && pm.Id == message.PatientMedicationId, 
                new string[] { "Concept.MedicationForm", "Product" } );

            if (patientMedicationFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate patient medication");
            }

            var mappedPatientMedication = _mapper.Map<PatientMedicationDetailDto>(patientMedicationFromRepo);

            await CustomMapAsync(patientMedicationFromRepo, mappedPatientMedication);

            return CreateLinks(mappedPatientMedication);
        }

        private async Task CustomMapAsync(PatientMedication patientMedicationFromRepo, PatientMedicationDetailDto dto)
        {
            IExtendable patientMedicationExtended = patientMedicationFromRepo;

            // Map all custom attributes
            dto.MedicationAttributes = _modelExtensionBuilder.BuildModelExtension(patientMedicationExtended)
                .Select(h => new AttributeValueDto()
                {
                    Id = h.Id,
                    Key = h.AttributeKey,
                    Value = h.TransformValueToString(),
                    Category = h.Category,
                    SelectionValue = GetSelectionValue(h.Type, h.AttributeKey, h.Value.ToString())
                }).Where(s => (s.Value != "0" && !String.IsNullOrWhiteSpace(s.Value)) || !String.IsNullOrWhiteSpace(s.SelectionValue)).ToList();

            dto.IndicationType = await _customAttributeService.GetCustomAttributeValueAsync("PatientMedication", "Type of Indication", patientMedicationExtended);
            dto.ReasonForStopping = await _customAttributeService.GetCustomAttributeValueAsync("PatientMedication", "Reason For Stopping", patientMedicationExtended);
            dto.ClinicianAction = await _customAttributeService.GetCustomAttributeValueAsync("PatientMedication", "Clinician action taken with regard to medicine if related to AE", patientMedicationExtended);
            dto.ChallengeEffect = await _customAttributeService.GetCustomAttributeValueAsync("PatientMedication", "Effect OF Dechallenge (D) & Rechallenge (R)", patientMedicationExtended);
        }

        private string GetSelectionValue(CustomAttributeType attributeType, string attributeKey, string selectionKey)
        {
            if (attributeType == CustomAttributeType.Selection)
            {
                var selectionitem = _selectionDataItemRepository.Get(s => s.AttributeKey == attributeKey && s.SelectionKey == selectionKey);

                return selectionitem == null ? string.Empty : selectionitem.Value;
            }

            return "";
        }

        private PatientMedicationDetailDto CreateLinks(PatientMedicationDetailDto mappedPatientMedication)
        {
            mappedPatientMedication.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("PatientMedication", mappedPatientMedication.Id), "self", "GET"));

            return mappedPatientMedication;
        }
    }
}
