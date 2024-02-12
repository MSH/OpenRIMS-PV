using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Exceptions;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.AttachmentAggregate
{
    public class AddAttachmentCommandHandler
        : IRequestHandler<AddAttachmentCommand, AttachmentIdentifierDto>
    {
        private readonly IRepositoryInt<AttachmentType> _attachmentTypeRepository;
        private readonly IRepositoryInt<Attachment> _attachmentRepository;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<AddAttachmentCommandHandler> _logger;

        public AddAttachmentCommandHandler(
            IRepositoryInt<AttachmentType> attachmentTypeRepository,
            IRepositoryInt<Attachment> attachmentRepository,
            IRepositoryInt<Patient> patientRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<AddAttachmentCommandHandler> logger)
        {
            _attachmentTypeRepository = attachmentTypeRepository ?? throw new ArgumentNullException(nameof(attachmentTypeRepository));
            _attachmentRepository = attachmentRepository ?? throw new ArgumentNullException(nameof(attachmentRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AttachmentIdentifierDto> Handle(AddAttachmentCommand message, CancellationToken cancellationToken)
        {
            Patient patientFromRepo = null;
            if (message.PatientId.HasValue)
            {
                patientFromRepo = await _patientRepository.GetAsync(f => f.Id == message.PatientId, new string[] { "" });
                if (patientFromRepo == null)
                {
                    throw new KeyNotFoundException("Unable to locate patient");
                }
            }

            if (message.Attachment.Length == 0)
            {
                throw new DomainException("Invalid file sent for attachment");
            }

            var fileExtension = Path.GetExtension(message.Attachment.FileName).Replace(".", "");
            var attachmentTypeFromRepo = await _attachmentTypeRepository.GetAsync(at => at.Key == fileExtension);
            if (attachmentTypeFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate attachment type {fileExtension}");
            }

            var fileName = ContentDispositionHeaderValue.Parse(message.Attachment.ContentDisposition).FileName.Trim();
            if (fileName.Length > 50)
            {
                throw new DomainException("Maximumum file name length of 50 characters, please rename the file before uploading");
            }

            // Create the attachment
            BinaryReader reader = new BinaryReader(message.Attachment.OpenReadStream());
            byte[] buffer = reader.ReadBytes((int)message.Attachment.Length);

            var newAttachment = new Attachment(buffer, message.Description, message.Attachment.FileName, message.Attachment.Length, attachmentTypeFromRepo, null, patientFromRepo, null);

            await _attachmentRepository.SaveAsync(newAttachment);

            _logger.LogInformation($"----- Attachment {message.Attachment.FileName} created");

            var mappedAttachment = _mapper.Map<AttachmentIdentifierDto>(newAttachment);

            return CreateLinks(mappedAttachment);
        }

        private AttachmentIdentifierDto CreateLinks(AttachmentIdentifierDto dto)
        {
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Attachment", dto.Id), "self", "GET"));

            return dto;
        }
    }
}
