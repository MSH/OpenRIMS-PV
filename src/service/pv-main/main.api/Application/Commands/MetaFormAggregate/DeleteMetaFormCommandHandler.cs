using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.Core.Aggregates.MetaFormAggregate;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.MetaFormAggregate
{
    public class DeleteMetaFormCommandHandler
        : IRequestHandler<DeleteMetaFormCommand, bool>
    {
        private readonly IRepositoryInt<MetaForm> _metaFormRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<DeleteMetaFormCommandHandler> _logger;

        public DeleteMetaFormCommandHandler(
            IRepositoryInt<MetaForm> metaFormRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<DeleteMetaFormCommandHandler> logger)
        {
            _metaFormRepository = metaFormRepository ?? throw new ArgumentNullException(nameof(metaFormRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(DeleteMetaFormCommand message, CancellationToken cancellationToken)
        {
            var metaFormFromRepo = await _metaFormRepository.GetAsync(c => c.Id == message.MetaFormId, new string[] {
                "Categories.Attributes.CustomAttributeConfiguration"
            });
            if (metaFormFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate meta form {message.MetaFormId}");
            }

            _metaFormRepository.Delete(metaFormFromRepo);

            _logger.LogInformation($"----- Meta form {message.Id} deleted");

            return await _unitOfWork.CompleteAsync();
        }
    }
}