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
    public class PatientClinicalEventDetailQueryHandler
        : IRequestHandler<PatientClinicalEventDetailQuery, PatientClinicalEventDetailDto>
    {
        private readonly IRepositoryInt<PatientClinicalEvent> _patientClinicalEventRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly ICustomAttributeService _customAttributeService;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientClinicalEventDetailQueryHandler> _logger;

        public PatientClinicalEventDetailQueryHandler(
            IRepositoryInt<PatientClinicalEvent> patientClinicalEventRepository,
            IRepositoryInt<SelectionDataItem> selectionDataItemRepository,
            ITypeExtensionHandler modelExtensionBuilder,
            ILinkGeneratorService linkGeneratorService,
            ICustomAttributeService customAttributeService,
            IMapper mapper,
            ILogger<PatientClinicalEventDetailQueryHandler> logger)
        {
            _patientClinicalEventRepository = patientClinicalEventRepository ?? throw new ArgumentNullException(nameof(patientClinicalEventRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _customAttributeService = customAttributeService ?? throw new ArgumentNullException(nameof(customAttributeService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PatientClinicalEventDetailDto> Handle(PatientClinicalEventDetailQuery message, CancellationToken cancellationToken)
        {
            var patientClinicalEventFromRepo = await _patientClinicalEventRepository.GetAsync(pm => pm.Patient.Id == message.PatientId && pm.Id == message.PatientClinicalEventId, 
                new string[] { "SourceTerminologyMedDra" } );

            if (patientClinicalEventFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate patient clinical event");
            }

            var mappedPatientClinicalEvent = _mapper.Map<PatientClinicalEventDetailDto>(patientClinicalEventFromRepo);

            await CustomMapAsync(patientClinicalEventFromRepo, mappedPatientClinicalEvent);

            return CreateLinks(mappedPatientClinicalEvent);
        }

        private async Task CustomMapAsync(PatientClinicalEvent patientClinicalEventFromRepo, PatientClinicalEventDetailDto dto)
        {
            IExtendable patientClinicalEventExtended = patientClinicalEventFromRepo;

            // Map all custom attributes
            dto.ClinicalEventAttributes = _modelExtensionBuilder.BuildModelExtension(patientClinicalEventExtended)
                .Select(h => new AttributeValueDto()
                {
                    Id = h.Id,
                    Key = h.AttributeKey,
                    Value = h.TransformValueToString(),
                    Category = h.Category,
                    SelectionValue = GetSelectionValue(h.Type, h.AttributeKey, h.Value.ToString())
                }).Where(s => (s.Value != "0" && !String.IsNullOrWhiteSpace(s.Value)) || !String.IsNullOrWhiteSpace(s.SelectionValue)).ToList();

            dto.ReportDate = await _customAttributeService.GetCustomAttributeValueAsync("PatientClinicalEvent", "Date of Report", patientClinicalEventExtended);
            dto.IsSerious = await _customAttributeService.GetCustomAttributeValueAsync("PatientClinicalEvent", "Is the adverse event serious?", patientClinicalEventExtended);
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

        private PatientClinicalEventDetailDto CreateLinks(PatientClinicalEventDetailDto mappedPatientClinicalEvent)
        {
            mappedPatientClinicalEvent.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("PatientClinicalEvent", mappedPatientClinicalEvent.Id), "self", "GET"));

            return mappedPatientClinicalEvent;
        }
    }
}
