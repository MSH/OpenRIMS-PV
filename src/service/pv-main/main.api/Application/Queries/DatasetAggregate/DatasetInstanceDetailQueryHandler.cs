using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Aggregates.DatasetAggregate;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Queries.DatasetAggregate
{
    public class DatasetInstanceDetailQueryHandler
        : IRequestHandler<DatasetInstanceDetailQuery, DatasetInstanceDetailDto>
    {
        private readonly IRepositoryInt<Dataset> _datasetRepository;
        private readonly IRepositoryInt<DatasetInstance> _datasetInstanceRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DatasetInstanceDetailQueryHandler> _logger;

        public DatasetInstanceDetailQueryHandler(
            IRepositoryInt<Dataset> datasetRepository,
            IRepositoryInt<DatasetInstance> datasetInstanceRepository,
            IMapper mapper,
            ILogger<DatasetInstanceDetailQueryHandler> logger)
        {
            _datasetRepository = datasetRepository ?? throw new ArgumentNullException(nameof(datasetRepository));
            _datasetInstanceRepository = datasetInstanceRepository ?? throw new ArgumentNullException(nameof(datasetInstanceRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<DatasetInstanceDetailDto> Handle(DatasetInstanceDetailQuery message, CancellationToken cancellationToken)
        {
            var datasetFromRepo = await _datasetRepository.GetAsync(d => d.Id == message.DatasetId);
            if (datasetFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate dataset");
            }

            var datasetInstanceFromRepo = await _datasetInstanceRepository.GetAsync(di => di.Dataset.Id == message.DatasetId 
                && di.Id == message.DatasetInstanceId,
                new string[] { "Dataset.DatasetCategories.DatasetCategoryElements"
                    , "Dataset.DatasetCategories.DatasetCategoryElements.DatasetElement.Field.FieldValues"
                    , "Dataset.DatasetCategories.DatasetCategoryElements.DatasetElement.Field.FieldType"
                    , "Dataset.DatasetCategories.DatasetCategoryElements.DatasetElement.DatasetElementSubs.Field.FieldValues"
                    , "Dataset.DatasetCategories.DatasetCategoryElements.DatasetElement.DatasetElementSubs.Field.FieldType"
                    , "Dataset.DatasetCategories.DatasetCategoryElements.DatasetCategoryElementConditions"
                    , "DatasetInstanceValues.DatasetInstanceSubValues"
                });
            if (datasetInstanceFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate dataset instance");
            }

            var mappedDatasetInstance = _mapper.Map<DatasetInstanceDetailDto>(datasetInstanceFromRepo);

            CustomMap(datasetInstanceFromRepo, mappedDatasetInstance);
            CreateLinks(mappedDatasetInstance);

            return mappedDatasetInstance;
        }

        private void CustomMap(DatasetInstance datasetInstanceFromRepo, DatasetInstanceDetailDto mappedDatasetInstanceDto)
        {
            var groupedDatasetCategories = datasetInstanceFromRepo.Dataset.DatasetCategories
                .SelectMany(dc => dc.DatasetCategoryElements).OrderBy(dc => dc.FieldOrder)
                .GroupBy(dce => dce.DatasetCategory)
                .ToList();

            mappedDatasetInstanceDto.DatasetCategories = groupedDatasetCategories
                .Select(dsc => new DatasetCategoryViewDto
                {
                    DatasetCategoryId = dsc.Key.Id,
                    DatasetCategoryName = dsc.Key.DatasetCategoryName,
                    DatasetCategoryDisplayName = dsc.Key.FriendlyName ?? dsc.Key.DatasetCategoryName,
                    DatasetCategoryHelp = dsc.Key.Help,
                    DatasetCategoryDisplayed = true,
                    DatasetElements = dsc.Select(element => new DatasetElementViewDto
                    {
                        DatasetElementId = element.DatasetElement.Id,
                        DatasetElementName = element.DatasetElement.ElementName,
                        DatasetElementDisplayName = element.FriendlyName ?? element.DatasetElement.ElementName,
                        DatasetElementHelp = element.Help,
                        DatasetElementDisplayed = true,
                        DatasetElementChronic = false,
                        DatasetElementSystem = element.DatasetElement.System,
                        DatasetElementType = element.DatasetElement.Field.FieldType.Description,
                        DatasetElementValue = datasetInstanceFromRepo.GetInstanceValue(element.DatasetElement.ElementName),
                        StringMaxLength = element.DatasetElement.Field.MaxLength,
                        NumericMinValue = element.DatasetElement.Field.MinSize,
                        NumericMaxValue = element.DatasetElement.Field.MaxSize,
                        Required = element.DatasetElement.Field.Mandatory,
                        SelectionDataItems = element.DatasetElement.Field.FieldValues.Select(fv => new SelectionDataItemDto() { SelectionKey = fv.Value, Value = fv.Value }).ToList(),
                        DatasetElementSubs = element.DatasetElement.DatasetElementSubs.Select(elementSub => new DatasetElementSubViewDto
                        {
                            DatasetElementSubId = elementSub.Id,
                            DatasetElementSubName = elementSub.ElementName,
                            DatasetElementSubType = elementSub.Field.FieldType.Description,
                            DatasetElementSubDisplayName = elementSub.FriendlyName ?? elementSub.ElementName,
                            DatasetElementSubHelp = elementSub.Help,
                            DatasetElementSubSystem = elementSub.System,
                            StringMaxLength = elementSub.Field.MaxLength,
                            NumericMinValue = elementSub.Field.MinSize,
                            NumericMaxValue = elementSub.Field.MaxSize,
                            Required = elementSub.Field.Mandatory,
                            SelectionDataItems = elementSub.Field.FieldValues.Select(fv => new SelectionDataItemDto() { SelectionKey = fv.Value, Value = fv.Value }).ToList(),
                        }).ToArray()
                    })
                    .ToArray()
                })
                .ToArray();
        }

        private void CreateLinks(DatasetInstanceDetailDto mappedDatasetInstanceDto)
        {
            //mappedDatasetInstanceDto.Links.Add(new LinkDto(CreateResourceUriHelper.CreateResourceUri(_urlHelper, "DatasetInstance", identifier.Id), "self", "GET"));
        }
    }
}
