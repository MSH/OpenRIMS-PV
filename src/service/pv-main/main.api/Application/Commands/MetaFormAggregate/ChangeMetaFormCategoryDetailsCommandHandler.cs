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
    public class ChangeMetaFormCategoryDetailsCommandHandler
        : IRequestHandler<ChangeMetaFormCategoryDetailsCommand, bool>
    {
        private readonly IRepositoryInt<MetaForm> _metaFormRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ChangeMetaFormCategoryDetailsCommandHandler> _logger;

        public ChangeMetaFormCategoryDetailsCommandHandler(
            IRepositoryInt<MetaForm> metaFormRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<ChangeMetaFormCategoryDetailsCommandHandler> logger)
        {
            _metaFormRepository = metaFormRepository ?? throw new ArgumentNullException(nameof(metaFormRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangeMetaFormCategoryDetailsCommand message, CancellationToken cancellationToken)
        {
            var metaFormFromRepo = await _metaFormRepository.GetAsync(c => c.Id == message.MetaFormId, new string[] { 
                "Categories" 
            });
            if (metaFormFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate meta form {message.MetaFormId}");
            }

            metaFormFromRepo.ChangeCategoryDetails(
                message.MetaFormCategoryId,
                message.CategoryName,
                message.Help,
                message.Icon);

            _metaFormRepository.Update(metaFormFromRepo);

            _logger.LogInformation($"----- Meta form category {message.MetaFormCategoryId} updated");

            return await _unitOfWork.CompleteAsync();
        }
    }
}
