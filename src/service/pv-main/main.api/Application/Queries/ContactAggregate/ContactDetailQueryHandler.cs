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
    public class ContactDetailQueryHandler
        : IRequestHandler<ContactDetailQuery, ContactDetailDto>
    {
        private readonly IRepositoryInt<SiteContactDetail> _siteContactDetailRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<ContactDetailQueryHandler> _logger;

        public ContactDetailQueryHandler(
            IRepositoryInt<SiteContactDetail> siteContactDetailRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<ContactDetailQueryHandler> logger)
        {
            _siteContactDetailRepository = siteContactDetailRepository ?? throw new ArgumentNullException(nameof(siteContactDetailRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ContactDetailDto> Handle(ContactDetailQuery message, CancellationToken cancellationToken)
        {
            var siteContactDetailFromRepo = await _siteContactDetailRepository.GetAsync(s => s.Id == message.SiteContactDetailId);

            if (siteContactDetailFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate contact detail");
            }

            var mappedContact = _mapper.Map<ContactDetailDto>(siteContactDetailFromRepo);

            CreateLinks(mappedContact);

            return mappedContact;
        }

        private void CreateLinks(ContactDetailDto mappedContact)
        {
            mappedContact.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Contact", mappedContact.Id), "self", "GET"));
        }
    }
}