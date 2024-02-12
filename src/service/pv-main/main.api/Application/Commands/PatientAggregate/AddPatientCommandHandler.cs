using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Exceptions;
using OpenRIMS.PV.Main.Core.Models;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.Services;
using OpenRIMS.PV.Main.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.PatientAggregate
{
    public class AddPatientCommandHandler
        : IRequestHandler<AddPatientCommand, PatientIdentifierDto>
    {
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly IRepositoryInt<CohortGroup> _cohortGroupRepository;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<EncounterType> _encounterTypeRepository;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<TerminologyMedDra> _terminologyMeddraRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IPatientService _patientService;
        private readonly IMapper _mapper;
        private readonly ILogger<AddPatientCommandHandler> _logger;

        public AddPatientCommandHandler(
            ITypeExtensionHandler modelExtensionBuilder,
            IRepositoryInt<CohortGroup> cohortGroupRepository,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<EncounterType> encounterTypeRepository,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<TerminologyMedDra> terminologyMeddraRepository,
            IUnitOfWorkInt unitOfWork,
            ILinkGeneratorService linkGeneratorService,
            IPatientService patientService,
            IMapper mapper,
            ILogger<AddPatientCommandHandler> logger)
        {
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _cohortGroupRepository = cohortGroupRepository ?? throw new ArgumentNullException(nameof(cohortGroupRepository));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _encounterTypeRepository = encounterTypeRepository ?? throw new ArgumentNullException(nameof(encounterTypeRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _terminologyMeddraRepository = terminologyMeddraRepository ?? throw new ArgumentNullException(nameof(terminologyMeddraRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _patientService = patientService ?? throw new ArgumentNullException(nameof(patientService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PatientIdentifierDto> Handle(AddPatientCommand message, CancellationToken cancellationToken)
        {
            await CheckIfPatientIsUniqueAsync(message.Attributes);
            await ValidateCommandModelAsync(message.MeddraTermId, message.CohortGroupId, message.EncounterTypeId);

            var patientDetail = await PreparePatientDetailAsync(message);
            if (!patientDetail.IsValid())
            {
                patientDetail.InvalidAttributes.ForEach(element => throw new DomainException(element));
            }

            var id = await _patientService.AddPatientAsync(patientDetail);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"----- Patient {message.LastName} created");

            var mappedPatient = await GetPatientAsync<PatientIdentifierDto>(id);
            if (mappedPatient == null)
            {
                throw new KeyNotFoundException("Unable to locate newly added patient");
            }

            return CreateLinks(mappedPatient);
        }

        private async Task CheckIfPatientIsUniqueAsync(IDictionary<int, string> attributes)
        {
            var medicalRecordNumberCustomAttribute = await _customAttributeRepository.GetAsync(ca => ca.AttributeKey == "Medical Record Number");
            var patientIdentityNumberCustomAttribute = await _customAttributeRepository.GetAsync(ca => ca.AttributeKey == "Patient Identity Number");

            List<CustomAttributeParameter> parameters = new List<CustomAttributeParameter>();
            if(medicalRecordNumberCustomAttribute != null)
            {
                if (attributes.ContainsKey(medicalRecordNumberCustomAttribute.Id))
                {
                    parameters.Add(new CustomAttributeParameter() { AttributeKey = "Medical Record Number", AttributeValue = attributes[medicalRecordNumberCustomAttribute.Id] });
                }
            }
            if (patientIdentityNumberCustomAttribute != null)
            {
                if (attributes.ContainsKey(patientIdentityNumberCustomAttribute.Id))
                {
                    parameters.Add(new CustomAttributeParameter() { AttributeKey = "Patient Identity Number", AttributeValue = attributes[patientIdentityNumberCustomAttribute.Id] });
                }
            }

            if (parameters.Count > 0)
            {
                if (!_patientService.isUnique(parameters))
                {
                    throw new DomainException("Potential duplicate patient. Check medical record number and patient identity number.");
                }
            }
        }

        private async Task ValidateCommandModelAsync(int meddraTermId, int? cohortGroupId, int encounterTypeId)
        {
            var sourceTermFromRepo = await _terminologyMeddraRepository.GetAsync(tm => tm.Id == meddraTermId);
            if (sourceTermFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate terminology for MedDRA");
            }

            if (cohortGroupId.HasValue)
            {
                if (cohortGroupId > 0)
                {
                    var cohortGroupFromRepo = await _cohortGroupRepository.GetAsync(cg => cg.Id == cohortGroupId.Value);
                    if (cohortGroupFromRepo == null)
                    {
                        throw new KeyNotFoundException("Unable to locate cohort group");
                    }
                }
            }

            var encounterTypeFromRepo = await _encounterTypeRepository.GetAsync(et => et.Id == encounterTypeId);
            if (encounterTypeFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate encounter type");
            }
        }

        private async Task<PatientDetailForCreation> PreparePatientDetailAsync(AddPatientCommand message)
        {
            var patientDetail = new PatientDetailForCreation(message.FirstName, message.LastName, message.MiddleName, message.FacilityName, string.Empty, message.DateOfBirth, message.CohortGroupId, message.EnroledDate, message.EncounterTypeId, message.PriorityId, message.EncounterDate);
            patientDetail.CustomAttributes = _modelExtensionBuilder.BuildModelExtension<Patient>();

            // Update patient custom attributes from source
            foreach (var newAttribute in message.Attributes)
            {
                var customAttribute = await _customAttributeRepository.GetAsync(ca => ca.Id == newAttribute.Key);
                if (customAttribute == null)
                {
                    throw new KeyNotFoundException($"Unable to locate custom attribute {newAttribute.Key}");
                }

                var attributeDetail = patientDetail.CustomAttributes.SingleOrDefault(ca => ca.AttributeKey == customAttribute.AttributeKey);

                if (attributeDetail == null)
                {
                    throw new KeyNotFoundException($"Unable to locate custom attribute on patient {newAttribute.Key}");
                }

                attributeDetail.Value = newAttribute.Value;
            }

            // Prepare primary condition
            var sourceTermFromRepo = await _terminologyMeddraRepository.GetAsync(tm => tm.Id == message.MeddraTermId);
            var conditionDetail = new ConditionDetail()
            {
                CustomAttributes = _modelExtensionBuilder.BuildModelExtension<PatientCondition>(),
                MeddraTermId = sourceTermFromRepo.Id,
                ConditionSource = sourceTermFromRepo.MedDraTerm,
                OnsetDate = message.StartDate,
                OutcomeDate = message.OutcomeDate,
                Comments = message.Comments,
                CaseNumber = message.CaseNumber
            };

            patientDetail.Conditions.Add(conditionDetail);

            return patientDetail;
        }

        private async Task<T> GetPatientAsync<T>(long id) where T : class
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == id);

            if (patientFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedPatient = _mapper.Map<T>(patientFromRepo);

                return mappedPatient;
            }

            return null;
        }

        private PatientIdentifierDto CreateLinks(PatientIdentifierDto dto)
        {
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Patient", dto.Id), "self", "GET"));
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateNewAppointmentForPatientResourceUri(dto.Id), "newAppointment", "POST"));
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateNewEnrolmentForPatientResourceUri(dto.Id), "newEnrolment", "POST"));

            return dto;
        }
    }
}
