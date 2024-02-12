using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
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
    public class PatientConditionDetailQueryHandler
        : IRequestHandler<PatientConditionDetailQuery, PatientConditionDetailDto>
    {
        private readonly IRepositoryInt<PatientCondition> _patientConditionRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientConditionDetailQueryHandler> _logger;

        public PatientConditionDetailQueryHandler(
            IRepositoryInt<PatientCondition> patientConditionRepository,
            IRepositoryInt<SelectionDataItem> selectionDataItemRepository,
            ITypeExtensionHandler modelExtensionBuilder,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<PatientConditionDetailQueryHandler> logger)
        {
            _patientConditionRepository = patientConditionRepository ?? throw new ArgumentNullException(nameof(patientConditionRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PatientConditionDetailDto> Handle(PatientConditionDetailQuery message, CancellationToken cancellationToken)
        {
            var patientConditionFromRepo = await _patientConditionRepository.GetAsync(pm => pm.Patient.Id == message.PatientId && pm.Id == message.PatientConditionId, 
                new string[] { "TerminologyMedDra", "Outcome", "TreatmentOutcome" } );

            if (patientConditionFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate patient condition");
            }

            var mappedPatientCondition = _mapper.Map<PatientConditionDetailDto>(patientConditionFromRepo);

            CustomMap(patientConditionFromRepo, mappedPatientCondition);

            return CreateLinks(mappedPatientCondition);
        }

        private void CustomMap(PatientCondition patientConditionFromRepo, PatientConditionDetailDto dto)
        {
            IExtendable patientConditionExtended = patientConditionFromRepo;

            // Map all custom attributes
            dto.ConditionAttributes = _modelExtensionBuilder.BuildModelExtension(patientConditionExtended)
                .Select(h => new AttributeValueDto()
                {
                    Key = h.AttributeKey,
                    Value = h.TransformValueToString(),
                    Category = h.Category,
                    SelectionValue = GetSelectionValue(h.Type, h.AttributeKey, h.Value.ToString())
                }).Where(s => (s.Value != "0" && !String.IsNullOrWhiteSpace(s.Value)) || !String.IsNullOrWhiteSpace(s.SelectionValue)).ToList();
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

        private PatientConditionDetailDto CreateLinks(PatientConditionDetailDto mappedPatientCondition)
        {
            mappedPatientCondition.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("PatientCondition", mappedPatientCondition.Id), "self", "GET"));

            return mappedPatientCondition;
        }
    }
}
