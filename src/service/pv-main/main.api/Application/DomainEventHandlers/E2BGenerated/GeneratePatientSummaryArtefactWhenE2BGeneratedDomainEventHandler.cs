using MediatR;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Events;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.DomainEventHandlers.E2BGenerated
{
    public class GeneratePatientSummaryArtefactWhenE2BGeneratedDomainEventHandler
                            : INotificationHandler<E2BGeneratedDomainEvent>
    {
        private readonly IRepositoryInt<AttachmentType> _attachmentTypeRepository;
        private readonly IArtefactService _artefactService;

        public GeneratePatientSummaryArtefactWhenE2BGeneratedDomainEventHandler(
            IRepositoryInt<AttachmentType> attachmentTypeRepository,
            IArtefactService artefactService)
        {
            _attachmentTypeRepository = attachmentTypeRepository ?? throw new ArgumentNullException(nameof(attachmentTypeRepository));
            _artefactService = artefactService ?? throw new ArgumentNullException(nameof(artefactService));
        }

        public async Task Handle(E2BGeneratedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var executionEvent = domainEvent.ReportInstance.CurrentActivity.GetLatestEvent();
            await CreatePatientSummaryAndLinkToExecutionEventAsync(domainEvent.ReportInstance, executionEvent);
        }

        private async Task CreatePatientSummaryAndLinkToExecutionEventAsync(ReportInstance reportInstance, ActivityExecutionStatusEvent executionEvent)
        {
            if (reportInstance is null)
            {
                throw new ArgumentNullException(nameof(reportInstance));
            }

            if (executionEvent is null)
            {
                throw new ArgumentNullException(nameof(executionEvent));
            }

            var artefactModel = reportInstance.WorkFlow.Description == "New Active Surveilliance Report" ?
                await _artefactService.CreatePatientSummaryForActiveReportAsync(reportInstance.ContextGuid) :
                await _artefactService.CreatePatientSummaryForSpontaneousReportAsync(reportInstance.ContextGuid);

            using (var tempFile = File.OpenRead(artefactModel.FullPath))
            {
                if (tempFile.Length > 0)
                {
                    BinaryReader rdr = new BinaryReader(tempFile);
                    executionEvent.AddAttachment(Path.GetFileName(artefactModel.FileName),
                        await _attachmentTypeRepository.GetAsync(at => at.Key == "docx"),
                        tempFile.Length,
                        rdr.ReadBytes((int)tempFile.Length),
                        "PatientSummary");
                }
            }
        }
    }
}
