using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Application.Queries.ReportInstanceAggregate;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;
using OpenRIMS.PV.Main.Core.CustomAttributes;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Queries.PatientAggregate
{
    public class PatientClinicalEventExpandedQueryHandler
        : IRequestHandler<PatientClinicalEventExpandedQuery, PatientClinicalEventExpandedDto>
    {
        private readonly IRepositoryInt<PatientClinicalEvent> _patientClinicalEventRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;
        private readonly IReportInstanceQueries _reportInstanceQueries;
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly ICustomAttributeService _customAttributeService;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientClinicalEventExpandedQueryHandler> _logger;

        public PatientClinicalEventExpandedQueryHandler(
            IRepositoryInt<PatientClinicalEvent> patientClinicalEventRepository,
            IRepositoryInt<SelectionDataItem> selectionDataItemRepository,
            IRepositoryInt<ReportInstance> reportInstanceRepository,
            IReportInstanceQueries reportInstanceQueries,
            ITypeExtensionHandler modelExtensionBuilder,
            ILinkGeneratorService linkGeneratorService,
            ICustomAttributeService customAttributeService,
            IMapper mapper,
            ILogger<PatientClinicalEventExpandedQueryHandler> logger)
        {
            _patientClinicalEventRepository = patientClinicalEventRepository ?? throw new ArgumentNullException(nameof(patientClinicalEventRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
            _reportInstanceQueries = reportInstanceQueries ?? throw new ArgumentNullException(nameof(reportInstanceQueries));
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _customAttributeService = customAttributeService ?? throw new ArgumentNullException(nameof(customAttributeService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PatientClinicalEventExpandedDto> Handle(PatientClinicalEventExpandedQuery message, CancellationToken cancellationToken)
        {
            var patientClinicalEventFromRepo = await _patientClinicalEventRepository.GetAsync(pm => pm.Patient.Id == message.PatientId && pm.Id == message.PatientClinicalEventId, 
                new string[] { "SourceTerminologyMedDra" } );

            if (patientClinicalEventFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate patient clinical event");
            }

            var mappedPatientClinicalEvent = _mapper.Map<PatientClinicalEventExpandedDto>(patientClinicalEventFromRepo);

            await CustomMapAsync(patientClinicalEventFromRepo, mappedPatientClinicalEvent);

            return CreateLinks(mappedPatientClinicalEvent);
        }

        private async Task CustomMapAsync(PatientClinicalEvent patientClinicalEventFromRepo, PatientClinicalEventExpandedDto dto)
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

            var activity = await _reportInstanceQueries.GetExecutionStatusEventsForEventViewAsync(patientClinicalEventFromRepo.Id);
            dto.Activity = activity.ToList();

            var reportInstanceFromRepo = await _reportInstanceRepository.GetAsync(ri => ri.ContextGuid == patientClinicalEventFromRepo.PatientClinicalEventGuid, new string[] { "TerminologyMedDra", "Medications", "Tasks.Comments" });
            if (reportInstanceFromRepo == null)
            {
                return;
            }

            dto.SetMedDraTerm = reportInstanceFromRepo.TerminologyMedDra?.DisplayName;
            dto.SetClassification = ReportClassification.From(reportInstanceFromRepo.ReportClassificationId).Name;
            dto.Medications = _mapper.Map<ICollection<ReportInstanceMedicationDetailDto>>(reportInstanceFromRepo.Medications.Where(m => !String.IsNullOrWhiteSpace(m.WhoCausality) || (!String.IsNullOrWhiteSpace(m.NaranjoCausality))));
            dto.Tasks = _mapper.Map<ICollection<TaskDto>>(reportInstanceFromRepo.Tasks.Where(t => t.TaskStatusId != Core.Aggregates.ReportInstanceAggregate.TaskStatus.Cancelled.Id));
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

        private PatientClinicalEventExpandedDto CreateLinks(PatientClinicalEventExpandedDto mappedPatientClinicalEvent)
        {
            mappedPatientClinicalEvent.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("PatientClinicalEvent", mappedPatientClinicalEvent.Id), "self", "GET"));

            return mappedPatientClinicalEvent;
        }
    }
}
