using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Aggregates.MetaFormAggregate;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Queries.MetaFormAggregate
{
    public class MetaFormExpandedQueryHandler
        : IRequestHandler<MetaFormExpandedQuery, MetaFormExpandedDto>
    {
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeConfigurationRepository;
        private readonly IRepositoryInt<MetaForm> _metaFormRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<MetaFormExpandedQueryHandler> _logger;

        public MetaFormExpandedQueryHandler(
            IRepositoryInt<CustomAttributeConfiguration> customAttributeConfigurationRepository,
            IRepositoryInt<MetaForm> metaFormRepository,
            IRepositoryInt<SelectionDataItem> selectionDataItemRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<MetaFormExpandedQueryHandler> logger)
        {
            _customAttributeConfigurationRepository = customAttributeConfigurationRepository ?? throw new ArgumentNullException(nameof(customAttributeConfigurationRepository));
            _metaFormRepository = metaFormRepository ?? throw new ArgumentNullException(nameof(metaFormRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<MetaFormExpandedDto> Handle(MetaFormExpandedQuery message, CancellationToken cancellationToken)
        {
            var metaFormFromRepo = await _metaFormRepository.GetAsync(mf => mf.Id == message.MetaFormId, new string[] { 
                "CohortGroup", 
                "Categories.MetaTable",
                "Categories.Attributes.CustomAttributeConfiguration",
            });

            if (metaFormFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate meta form");
            }

            var mappedMetaForm = _mapper.Map<MetaFormExpandedDto>(metaFormFromRepo);

            await CustomMapAsync(mappedMetaForm, message.IncludeUnmappedAttributes);

            return CreateLinks(mappedMetaForm);
        }

        private async Task CustomMapAsync(MetaFormExpandedDto mappedMetaForm, bool includeUnmappedAttributes)
        {
            if (mappedMetaForm == null)
            {
                throw new ArgumentNullException(nameof(mappedMetaForm));
            }

            foreach (var category in mappedMetaForm.Categories)
            {
                await CustomCategoryMapAsync(category, includeUnmappedAttributes);
            }

        }

        private async Task CustomCategoryMapAsync(MetaFormCategoryDto category, bool includeUnmappedAttributes)
        {
            if(category.MetaTableName == "Patient")
            {
                await ProcessEntityAsync(typeof(Patient), "Patient", category, includeUnmappedAttributes);
            }

            if (category.MetaTableName == "PatientMedication")
            {
                await ProcessEntityAsync(typeof(PatientMedication), "PatientMedication", category, includeUnmappedAttributes);
            }

            if (category.MetaTableName == "PatientClinicalEvent")
            {
                await ProcessEntityAsync(typeof(PatientClinicalEvent), "PatientClinicalEvent", category, includeUnmappedAttributes);
            }

            if (category.MetaTableName == "PatientCondition")
            {
                await ProcessEntityAsync(typeof(PatientCondition), "PatientCondition", category, includeUnmappedAttributes);
            }

            if (category.MetaTableName == "PatientLabTest")
            {
                await ProcessEntityAsync(typeof(PatientLabTest), "PatientLabTest", category, includeUnmappedAttributes);
            }

            if (category.MetaTableName == "Encounter")
            {
                await ProcessEntityAsync(typeof(Encounter), "Encounter", category, includeUnmappedAttributes);
            }
        }

        private MetaFormExpandedDto CreateLinks(MetaFormExpandedDto mappedMetaForm)
        {
            mappedMetaForm.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("MetaForm", mappedMetaForm.Id), "self", "GET"));

            return mappedMetaForm;
        }

        private async Task ProcessEntityAsync(Type type, string entityName, MetaFormCategoryDto category, bool includeUnmappedAttributes)
        {
            foreach (var attribute in category.Attributes.Where(a => a.FormAttributeType == "DropDownList"))
            {
                CreateSelectionValues(attribute);
            }

            // include unmapped fields if required
            if (includeUnmappedAttributes)
            {
                PropertyInfo[] properties = type.GetProperties();
                var invalidProperties = new[] {
                    "CustomAttributesXmlSerialised",
                    "Archived",
                    "ArchivedReason",
                    "ArchivedDate",
                    "AuditUser",
                    "AuditUserId",
                    "Age",
                    "FullName",
                    "DisplayName",
                    "CurrentFacilityName",
                    "LatestEncounterDate",
                    "AgeGroup",
                    "Created",
                    "CreatedBy",
                    "CreatedById",
                    "Attachments",
                    "LastUpdated",
                    "UpdatedBy",
                    "UpdatedById",
                    "DomainEvents",
                    "PatientLanguages"
                };

                var attributes = await _customAttributeConfigurationRepository.ListAsync(c => c.ExtendableTypeName == entityName);

                foreach (PropertyInfo property in properties)
                {
                    if (!invalidProperties.Contains(property.Name) && !category.Attributes.Any(a => a.AttributeName == property.Name))
                    {
                        category.UnmappedAttributes.Add(new MetaFormCategoryAttributeDto()
                        {
                            Selected = false,
                            AttributeName = property.Name,
                            MetaFormCategoryAttributeGuid = Guid.NewGuid(),
                            FormAttributeType = "AlphaNumericTextbox",
                            Required = false,
                            StringMaxLength = 50
                        });
                    }
                }

                // Now process attributes
                foreach (CustomAttributeConfiguration sourceAttribute in attributes)
                {
                    if (!category.Attributes.Any(a => a.AttributeId == sourceAttribute.Id))
                    {
                        category.UnmappedAttributes.Add(new MetaFormCategoryAttributeDto()
                        {
                            Selected = false,
                            AttributeId = sourceAttribute.Id,
                            AttributeName = sourceAttribute.AttributeKey,
                            MetaFormCategoryAttributeGuid = Guid.NewGuid(),
                            FormAttributeType = "AlphaNumericTextbox",
                            Required = false,
                            StringMaxLength = 50
                        });
                    }
                }
            }
        }

        private void CreateSelectionValues(MetaFormCategoryAttributeDto dto)
        {
            dto.SelectionDataItems = _selectionDataItemRepository.List(s => s.AttributeKey == dto.AttributeName, null, "")
                .Select(ss => new SelectionDataItemDto()
                {
                    SelectionKey = ss.SelectionKey,
                    Value = ss.Value
                })
                .ToList();
        }
    }
}
