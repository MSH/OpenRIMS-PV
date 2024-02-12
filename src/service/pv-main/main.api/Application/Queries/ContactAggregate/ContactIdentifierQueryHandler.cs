using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Aggregates.ContactAggregate;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Queries.ContactAggregate
{
    public class ContactIdentifierQueryHandler
        : IRequestHandler<ContactIdentifierQuery, ContactIdentifierDto>
    {
        private readonly IRepositoryInt<SiteContactDetail> _siteContactDetailRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<ContactIdentifierQueryHandler> _logger;

        public ContactIdentifierQueryHandler(
            IRepositoryInt<SiteContactDetail> siteContactDetailRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<ContactIdentifierQueryHandler> logger)
        {
            _siteContactDetailRepository = siteContactDetailRepository ?? throw new ArgumentNullException(nameof(siteContactDetailRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ContactIdentifierDto> Handle(ContactIdentifierQuery message, CancellationToken cancellationToken)
        {
            var siteContactDetailFromRepo = await _siteContactDetailRepository.GetAsync(s => s.Id == message.SiteContactDetailId);

            if (siteContactDetailFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate contact detail");
            }

            var mappedContact = _mapper.Map<ContactIdentifierDto>(siteContactDetailFromRepo);

            CreateLinks(mappedContact);

            return mappedContact;
        }

        private void CreateLinks(ContactIdentifierDto mappedContact)
        {
            mappedContact.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Contact", mappedContact.Id), "self", "GET"));
        }
    }
}
