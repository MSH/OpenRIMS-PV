using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.Core.CustomAttributes;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Exceptions;
using OpenRIMS.PV.Main.Core.Models;
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
    public class ChangePatientCustomAttributesCommandHandler
        : IRequestHandler<ChangePatientCustomAttributesCommand, bool>
    {
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ChangePatientCustomAttributesCommandHandler> _logger;
        private readonly IPatientService _patientService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChangePatientCustomAttributesCommandHandler(
            ITypeExtensionHandler modelExtensionBuilder,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<Patient> patientRepository,
            IPatientService patientService,
            IUnitOfWorkInt unitOfWork,
            ILogger<ChangePatientCustomAttributesCommandHandler> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _patientService = patientService ?? throw new ArgumentNullException(nameof(patientService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<bool> Handle(ChangePatientCustomAttributesCommand message, CancellationToken cancellationToken)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == message.PatientId);
            if (patientFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate patient");
            }

            await CheckIfPatientIsStillUniqueAsync(message.PatientId, message.Attributes);

            var patientAttributes = await PreparePatientAttributesWithNewValuesAsync(patientFromRepo, message.Attributes);

            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            _modelExtensionBuilder.ValidateAndUpdateExtendable(patientFromRepo, patientAttributes, userName);

            _patientRepository.Update(patientFromRepo);

            _logger.LogInformation($"----- Patient {message.PatientId} custom attributes details updated");

            return await _unitOfWork.CompleteAsync();
        }

        private async Task<List<CustomAttributeDetail>> PreparePatientAttributesWithNewValuesAsync(IExtendable extendedEntity, IDictionary<int, string> newAttributes)
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

        private async Task CheckIfPatientIsStillUniqueAsync(int patientId, IDictionary<int, string> attributes)
        {
            var medicalRecordNumberCustomAttribute = await _customAttributeRepository.GetAsync(ca => ca.AttributeKey == "Medical Record Number");
            var patientIdentityNumberCustomAttribute = await _customAttributeRepository.GetAsync(ca => ca.AttributeKey == "Patient Identity Number");

            List<CustomAttributeParameter> parameters = new List<CustomAttributeParameter>();
            if (medicalRecordNumberCustomAttribute != null)
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
                if (!_patientService.isUnique(parameters, patientId))
                {
                    throw new DomainException("Potential duplicate patient. Check medical record number and patient identity number.");
                }
            }
        }
    }
}