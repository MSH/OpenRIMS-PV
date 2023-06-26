using MediatR;
using PVIMS.API.Infrastructure.Services;
using PVIMS.Core.Aggregates.DatasetAggregate;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;
using PVIMS.Core.Entities;
using PViMS.Core.Events;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.DomainEventHandlers.E2BGenerated
{
    public class GenerateE2BExtractArtefactWhenE2BGeneratedDomainEventHandler
                            : INotificationHandler<E2BGeneratedDomainEvent>
    {
        private readonly IRepositoryInt<DatasetInstance> _datasetInstanceRepository;
        private readonly IXmlDocumentService _xmlDocumentService;
        private readonly IUnitOfWorkInt _unitOfWork;

        public GenerateE2BExtractArtefactWhenE2BGeneratedDomainEventHandler(
            IRepositoryInt<DatasetInstance> datasetInstanceRepository,
            IXmlDocumentService xmlDocumentService,
            IUnitOfWorkInt unitOfWork)
        {
            _datasetInstanceRepository = datasetInstanceRepository ?? throw new ArgumentNullException(nameof(datasetInstanceRepository));
            _xmlDocumentService = xmlDocumentService ?? throw new ArgumentNullException(nameof(xmlDocumentService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task Handle(E2BGeneratedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var executionEvent = domainEvent.ReportInstance.CurrentActivity.GetLatestEvent();
            await CreateE2BExtractAndLinkToExecutionEventAsync(domainEvent.ReportInstance, executionEvent);
        }

        private async Task CreateE2BExtractAndLinkToExecutionEventAsync(ReportInstance reportInstance, ActivityExecutionStatusEvent executionEvent)
        {
            if (reportInstance is null)
            {
                throw new ArgumentNullException(nameof(reportInstance));
            }
            if (executionEvent is null)
            {
                throw new ArgumentNullException(nameof(executionEvent));
            }

            var e2bInstance = await GetDatasetInstance(reportInstance);
            if (e2bInstance == null)
            {
                throw new KeyNotFoundException($"Unable to locate E2B dataset for report {reportInstance.Id} for XML generation");
            }

            var artefactModel = await _xmlDocumentService.CreateE2BDocumentAsync(e2bInstance);

            using (var tempFile = File.OpenRead(artefactModel.FullPath))
            {
                if (tempFile.Length > 0)
                {
                    BinaryReader rdr = new BinaryReader(tempFile);
                    executionEvent.AddAttachment(Path.GetFileName(artefactModel.FileName),
                        _unitOfWork.Repository<AttachmentType>().Queryable().Single(at => at.Key == "xml"),
                        tempFile.Length,
                        rdr.ReadBytes((int)tempFile.Length),
                        "E2b");
                }
            }
        }

        private async Task<DatasetInstance> GetDatasetInstance(ReportInstance reportInstance)
        {
            var e2bInitiatedEvent = reportInstance.CurrentActivity.ExecutionEvents.OrderByDescending(ee => ee.EventDateTime)
                .First(ee => ee.ExecutionStatus.Description == "E2BINITIATED");
            if(e2bInitiatedEvent == null)
            {
                return null;
            }
            var tag = reportInstance.WorkFlow?.Description == "New Active Surveilliance Report" ? "Active" : "Spontaneous";
            return await _datasetInstanceRepository.GetAsync(di => di.Tag == tag && di.ContextId == e2bInitiatedEvent.Id, 
                new string[] { 
                    "Dataset.DatasetXml.ChildrenNodes.NodeAttributes.DatasetElement",
                    "DatasetInstanceValues.DatasetElement",
                    "DatasetInstanceValues.DatasetInstanceSubValues.DatasetElementSub"
                });
        }
    }
}
