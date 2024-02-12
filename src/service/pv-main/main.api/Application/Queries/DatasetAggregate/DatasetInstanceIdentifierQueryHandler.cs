using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Aggregates.DatasetAggregate;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Queries.DatasetAggregate
{
    public class DatasetInstanceIdentifierQueryHandler
        : IRequestHandler<DatasetInstanceIdentifierQuery, DatasetInstanceIdentifierDto>
    {
        private readonly IRepositoryInt<Dataset> _datasetRepository;
        private readonly IRepositoryInt<DatasetInstance> _datasetInstanceRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DatasetInstanceIdentifierQueryHandler> _logger;

        public DatasetInstanceIdentifierQueryHandler(
            IRepositoryInt<Dataset> datasetRepository,
            IRepositoryInt<DatasetInstance> datasetInstanceRepository,
            IMapper mapper,
            ILogger<DatasetInstanceIdentifierQueryHandler> logger)
        {
            _datasetRepository = datasetRepository ?? throw new ArgumentNullException(nameof(datasetRepository));
            _datasetInstanceRepository = datasetInstanceRepository ?? throw new ArgumentNullException(nameof(datasetInstanceRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<DatasetInstanceIdentifierDto> Handle(DatasetInstanceIdentifierQuery message, CancellationToken cancellationToken)
        {
            var datasetFromRepo = await _datasetRepository.GetAsync(d => d.Id == message.DatasetId);
            if (datasetFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate dataset");
            }

            var datasetInstanceFromRepo = await _datasetInstanceRepository.GetAsync(di => di.Dataset.Id == message.DatasetId 
                && di.Id == message.DatasetInstanceId);
            if (datasetInstanceFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate dataset instance");
            }

            var mappedDatasetInstance = _mapper.Map<DatasetInstanceIdentifierDto>(datasetInstanceFromRepo);

            CreateLinks(mappedDatasetInstance);

            return mappedDatasetInstance;
        }

        private void CreateLinks(DatasetInstanceIdentifierDto mappedDatasetInstanceDto)
        {
            //mappedDatasetInstanceDto.Links.Add(new LinkDto(CreateResourceUriHelper.CreateResourceUri(_urlHelper, "DatasetInstance", identifier.Id), "self", "GET"));
        }
    }
}
