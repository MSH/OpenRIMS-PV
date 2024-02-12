using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Aggregates.MetaFormAggregate;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Queries.MetaFormAggregate
{
    public class MetaFormExpandedQueryHandler
        : IRequestHandler<MetaFormExpandedQuery, MetaFormExpandedDto>
    {
        private readonly IRepositoryInt<MetaForm> _metaFormRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<MetaFormExpandedQueryHandler> _logger;

        public MetaFormExpandedQueryHandler(
            IRepositoryInt<MetaForm> metaFormRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<MetaFormExpandedQueryHandler> logger)
        {
            _metaFormRepository = metaFormRepository ?? throw new ArgumentNullException(nameof(metaFormRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<MetaFormExpandedDto> Handle(MetaFormExpandedQuery message, CancellationToken cancellationToken)
        {
            var metaFormFromRepo = await _metaFormRepository.GetAsync(mf => mf.Id == message.MetaFormId, new string[] { 
                "CohortGroup", 
                "Categories.MetaTable",
                "Categories.Attributes",
            });

            if (metaFormFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate meta form");
            }

            var mappedMetaForm = _mapper.Map<MetaFormExpandedDto>(metaFormFromRepo);

            return CreateLinks(mappedMetaForm);
        }

        private MetaFormExpandedDto CreateLinks(MetaFormExpandedDto mappedMetaForm)
        {
            mappedMetaForm.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("MetaForm", mappedMetaForm.Id), "self", "GET"));

            return mappedMetaForm;
        }
    }
}
