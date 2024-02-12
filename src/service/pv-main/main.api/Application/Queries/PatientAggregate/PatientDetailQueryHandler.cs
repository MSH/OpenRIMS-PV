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
    public class PatientDetailQueryHandler
        : IRequestHandler<PatientDetailQuery, PatientDetailDto>
    {
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientDetailQueryHandler> _logger;

        public PatientDetailQueryHandler(
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<SelectionDataItem> selectionDataItemRepository,
            ITypeExtensionHandler modelExtensionBuilder,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<PatientDetailQueryHandler> logger)
        {
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PatientDetailDto> Handle(PatientDetailQuery message, CancellationToken cancellationToken)
        {
            var patientFromRepo = await _patientRepository.GetAsync(p => p.Archived == false
                    && p.Id == message.PatientId,
                    new string[] { "PatientFacilities.Facility.OrgUnit" });

            if (patientFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate patient");
            }

            var mappedPatient = _mapper.Map<PatientDetailDto>(patientFromRepo);

            CustomMap(patientFromRepo, mappedPatient);
            CreateLinks(mappedPatient);

            return mappedPatient;
        }

        private void CustomMap(Patient patientFromRepo, PatientDetailDto mappedPatient)
        {
            IExtendable patientExtended = patientFromRepo;

            mappedPatient.PatientAttributes = _modelExtensionBuilder.BuildModelExtension(patientExtended)
                .Select(h => new AttributeValueDto()
                {
                    Key = h.AttributeKey,
                    Value = h.Value.ToString(),
                    Category = h.Category,
                    SelectionValue = GetSelectionValue(h.Type, h.AttributeKey, h.Value.ToString())
                }).Where(s => (s.Value != "0" && !String.IsNullOrWhiteSpace(s.Value)) || !String.IsNullOrWhiteSpace(s.SelectionValue)).ToList();

            // Map additional attributes to main dto
            var attribute = patientExtended.GetAttributeValue("Medical Record Number");
            mappedPatient.MedicalRecordNumber = attribute != null ? attribute.ToString() : "";
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

        private void CreateLinks(PatientDetailDto mappedPatient)
        {
            mappedPatient.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Patient", mappedPatient.Id), "self", "GET"));
            mappedPatient.Links.Add(new LinkDto(_linkGeneratorService.CreateNewAppointmentForPatientResourceUri(mappedPatient.Id), "newAppointment", "POST"));
            mappedPatient.Links.Add(new LinkDto(_linkGeneratorService.CreateNewEnrolmentForPatientResourceUri(mappedPatient.Id), "newEnrolment", "POST"));
        }
    }
}
