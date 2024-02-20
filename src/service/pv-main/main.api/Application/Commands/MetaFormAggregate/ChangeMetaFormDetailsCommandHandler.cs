using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Application.Commands.MetaFormAggregate;
using OpenRIMS.PV.Main.Core.Aggregates.MetaFormAggregate;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.CohortGroupAggregate
{
    public class ChangeMetaFormDetailsCommandHandler
        : IRequestHandler<ChangeMetaFormDetailsCommand, bool>
    {
        private readonly IRepositoryInt<MetaForm> _metaFormRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ChangeMetaFormDetailsCommandHandler> _logger;

        public ChangeMetaFormDetailsCommandHandler(
            IRepositoryInt<MetaForm> metaFormRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<ChangeMetaFormDetailsCommandHandler> logger)
        {
            _metaFormRepository = metaFormRepository ?? throw new ArgumentNullException(nameof(metaFormRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangeMetaFormDetailsCommand message, CancellationToken cancellationToken)
        {
            var metaFormFromRepo = await _metaFormRepository.GetAsync(c => c.Id == message.MetaFormId, new string[] { ""
            });
            if (metaFormFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate meta form {message.MetaFormId}");
            }

            metaFormFromRepo.ChangeDetails(
                message.FormName,
                message.ActionName);

            _metaFormRepository.Update(metaFormFromRepo);

            _logger.LogInformation($"----- Meta form {message.MetaFormId} updated");

            return await _unitOfWork.CompleteAsync();
        }
    }
}
