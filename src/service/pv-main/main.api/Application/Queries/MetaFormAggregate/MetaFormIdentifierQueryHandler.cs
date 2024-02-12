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
    public class MetaFormIdentifierQueryHandler
        : IRequestHandler<MetaFormIdentifierQuery, MetaFormIdentifierDto>
    {
        private readonly IRepositoryInt<MetaForm> _metaFormRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<MetaFormIdentifierQueryHandler> _logger;

        public MetaFormIdentifierQueryHandler(
            IRepositoryInt<MetaForm> metaFormRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<MetaFormIdentifierQueryHandler> logger)
        {
            _metaFormRepository = metaFormRepository ?? throw new ArgumentNullException(nameof(metaFormRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<MetaFormIdentifierDto> Handle(MetaFormIdentifierQuery message, CancellationToken cancellationToken)
        {
            var metaFormFromRepo = await _metaFormRepository.GetAsync(mf => mf.Id == message.MetaFormId);

            if (metaFormFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate meta form");
            }

            var mappedMetaForm = _mapper.Map<MetaFormIdentifierDto>(metaFormFromRepo);

            return CreateLinks(mappedMetaForm);
        }

        private MetaFormIdentifierDto CreateLinks(MetaFormIdentifierDto mappedMetaForm)
        {
            mappedMetaForm.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("MetaForm", mappedMetaForm.Id), "self", "GET"));

            return mappedMetaForm;
        }
    }
}
