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
    public class ChangeLabTestDetailsCommandHandler
        : IRequestHandler<ChangeLabTestDetailsCommand, bool>
    {
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<LabTest> _labTestRepository;
        private readonly IRepositoryInt<LabTestUnit> _labTestUnitRepository;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ChangeLabTestDetailsCommandHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChangeLabTestDetailsCommandHandler(
            ITypeExtensionHandler modelExtensionBuilder,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<LabTest> labTestRepository,
            IRepositoryInt<LabTestUnit> labTestUnitRepository,
            IRepositoryInt<Patient> patientRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<ChangeLabTestDetailsCommandHandler> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _labTestRepository = labTestRepository ?? throw new ArgumentNullException(nameof(labTestRepository));
            _labTestUnitRepository = labTestUnitRepository ?? throw new ArgumentNullException(nameof(labTestUnitRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<bool> Handle(ChangeLabTestDetailsCommand message, CancellationToken cancellationToken)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == message.PatientId, new string[] {
                "PatientLabTests.LabTest",
                "PatientLabTests.TestUnit"
            });
            if (patientFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate patient {message.PatientId}");
            }

            LabTestUnit labTestUnitFromRepo = null;
            if (!String.IsNullOrWhiteSpace(message.TestUnit))
            {
                labTestUnitFromRepo = _labTestUnitRepository.Get(u => u.Description == message.TestUnit);
                if (labTestUnitFromRepo == null)
                {
                    throw new KeyNotFoundException($"Unable to locate lab test unit with a description of {message.TestUnit}");
                }
            }

            var labTestToUpdate = patientFromRepo.PatientLabTests.Single(plt => plt.Id == message.PatientLabTestId);
            var labTestAttributes = await PrepareAttributesWithNewValuesAsync(labTestToUpdate, message.Attributes);
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            patientFromRepo.ChangeLabTestDetails(message.PatientLabTestId, message.TestDate, message.TestResultCoded, labTestUnitFromRepo, message.TestResultValue, message.ReferenceLower, message.ReferenceUpper);

            _modelExtensionBuilder.ValidateAndUpdateExtendable(labTestToUpdate, labTestAttributes, userName);

            _patientRepository.Update(patientFromRepo);

            _logger.LogInformation($"----- Lab Test {message.PatientLabTestId} details updated");

            return await _unitOfWork.CompleteAsync();
        }

        private async Task<List<CustomAttributeDetail>> PrepareAttributesWithNewValuesAsync(IExtendable extendedEntity, IDictionary<int, string> newAttributes)
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