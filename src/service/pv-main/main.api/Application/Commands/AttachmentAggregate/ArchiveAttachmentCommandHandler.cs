using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.AttachmentAggregate
{
    public class ArchiveAttachmentCommandHandler
        : IRequestHandler<ArchiveAttachmentCommand, bool>
    {
        private readonly IRepositoryInt<Attachment> _attachmentRepository;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ArchiveAttachmentCommandHandler> _logger;

        public ArchiveAttachmentCommandHandler(
            IRepositoryInt<Attachment> attachmentRepository,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<User> userRepository,
            IUnitOfWorkInt unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ArchiveAttachmentCommandHandler> logger)
        {
            _attachmentRepository = attachmentRepository ?? throw new ArgumentNullException(nameof(attachmentRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ArchiveAttachmentCommand message, CancellationToken cancellationToken)
        {
            Patient patientFromRepo = null;
            if (message.PatientId.HasValue)
            {
                patientFromRepo = await _patientRepository.GetAsync(f => f.Id == message.PatientId);
                if (patientFromRepo == null)
                {
                    throw new KeyNotFoundException("Unable to locate patient");
                }
            }

            var attachmentFromRepo = await _attachmentRepository.GetAsync(f => f.Id == message.AttachmentId, new string[] { "AttachmentType" });
            if (patientFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate attachment");
            }

            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userFromRepo = await _userRepository.GetAsync(u => u.UserName == userName);
            if (userFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate user");
            }

            attachmentFromRepo.ArchiveAttachment(userFromRepo, message.Reason);
            _patientRepository.Update(patientFromRepo);

            _logger.LogInformation($"----- Clinical Event {message.AttachmentId} archived");

            return await _unitOfWork.CompleteAsync();
        }
    }
}
