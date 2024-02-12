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
    public class PatientLabTestDetailQueryHandler
        : IRequestHandler<PatientLabTestDetailQuery, PatientLabTestDetailDto>
    {
        private readonly IRepositoryInt<PatientLabTest> _patientLabTestRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientLabTestDetailQueryHandler> _logger;

        public PatientLabTestDetailQueryHandler(
            IRepositoryInt<PatientLabTest> patientLabTestRepository,
            IRepositoryInt<SelectionDataItem> selectionDataItemRepository,
            ITypeExtensionHandler modelExtensionBuilder,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<PatientLabTestDetailQueryHandler> logger)
        {
            _patientLabTestRepository = patientLabTestRepository ?? throw new ArgumentNullException(nameof(patientLabTestRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PatientLabTestDetailDto> Handle(PatientLabTestDetailQuery message, CancellationToken cancellationToken)
        {
            var patientLabTestFromRepo = await _patientLabTestRepository.GetAsync(plt => plt.Patient.Id == message.PatientId && plt.Id == message.PatientLabTestId, new string[] {
                "LabTest",
                "TestUnit"
            });

            if (patientLabTestFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate patient lab test {message.PatientLabTestId} for patient {message.PatientId}");
            }
                
            var mappedPatientLabTest = _mapper.Map<PatientLabTestDetailDto>(patientLabTestFromRepo);

            CustomMap(patientLabTestFromRepo, mappedPatientLabTest);
            CreateLinks(mappedPatientLabTest);

            return mappedPatientLabTest;
        }

        private void CustomMap(PatientLabTest patientLabTestFromRepo, PatientLabTestDetailDto dto)
        {
            IExtendable patientLabTestExtended = patientLabTestFromRepo;

            // Map all custom attributes
            dto.LabTestAttributes = _modelExtensionBuilder.BuildModelExtension(patientLabTestExtended)
                .Select(h => new AttributeValueDto()
                {
                    Id = h.Id,
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

        private void CreateLinks(PatientLabTestDetailDto mappedPatientLabTest)
        {
            mappedPatientLabTest.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("PatientLabTest", mappedPatientLabTest.Id), "self", "GET"));
        }
    }
}
