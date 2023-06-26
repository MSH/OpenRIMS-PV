using MediatR;
using MimeKit;
using PViMS.Core.Events;
using PVIMS.API.Infrastructure.Services;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.DomainEventHandlers.TaskCancelled
{
    public class SendEmailWhenTaskCancelledDomainEventHandler
                            : INotificationHandler<TaskCancelledDomainEvent>
    {
        private readonly ISMTPMailService _smtpMailService;
        private readonly ICustomAttributeService _attributeService;
        private readonly IRepositoryInt<PatientClinicalEvent> _patientClinicalEventRepository;

        public SendEmailWhenTaskCancelledDomainEventHandler(ISMTPMailService smtpMailService,
            ICustomAttributeService attributeService,
            IRepositoryInt<PatientClinicalEvent> patientClinicalEventRepository)
        {
            _smtpMailService = smtpMailService ?? throw new ArgumentNullException(nameof(smtpMailService));
            _attributeService = attributeService ?? throw new ArgumentNullException(nameof(attributeService));
            _patientClinicalEventRepository = patientClinicalEventRepository ?? throw new ArgumentNullException(nameof(patientClinicalEventRepository));
        }

        public async Task Handle(TaskCancelledDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var subject = $"Report task: {domainEvent.Task.ReportInstance.Identifier}";

            var sb = new StringBuilder();
            sb.Append($"Please note task {domainEvent.Task.ReportInstance.Identifier} has been cancelled: ");
            sb.Append("<p><b><u>Adverse Event Details</u></b></p>");
            sb.Append("<table>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Identifier</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.Task.ReportInstance.Identifier}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Patient</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.Task.ReportInstance.PatientIdentifier}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Created</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.Task.ReportInstance.Created}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Description</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.Task.ReportInstance.SourceIdentifier}</td></tr>");
            sb.Append("</table>");
            sb.Append("<p><b><u>Task Details</u></b></p>");
            sb.Append("<table>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Source</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.Task.TaskDetail.Source}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Description</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.Task.TaskDetail.Description}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Status</b></td><td style='padding: 10px; border: 1px solid black;'>{Core.Aggregates.ReportInstanceAggregate.TaskStatus.From(domainEvent.Task.TaskStatusId).Name}</td></tr>");
            sb.Append("</table>");
            sb.Append("<p><b>*** This is system generated. Please do not reply to this message ***</b></p>");

            await _smtpMailService.SendEmailAsync(subject, sb.ToString(), await PrepareDestinationMailBoxesAsync(domainEvent));
        }

        private async Task<List<MailboxAddress>> PrepareDestinationMailBoxesAsync(TaskCancelledDomainEvent domainEvent)
        {
            var patientClinicalEvent = await _patientClinicalEventRepository.GetAsync(pce => pce.PatientClinicalEventGuid == domainEvent.Task.ReportInstance.ContextGuid,
                new string[] { });
            if (patientClinicalEvent == null)
            {
                throw new KeyNotFoundException(nameof(patientClinicalEvent));
            }

            var extendable = (IExtendable)patientClinicalEvent;
            var reporterName = await _attributeService.GetCustomAttributeValueAsync("PatientClinicalEvent", "Name of reporter", extendable);
            var reporterEmail = await _attributeService.GetCustomAttributeValueAsync("PatientClinicalEvent", "Email address", extendable);

            var destinationAddresses = new List<MailboxAddress>();
            destinationAddresses.Add(new MailboxAddress(domainEvent.Task.ReportInstance.CreatedBy.FullName, domainEvent.Task.ReportInstance.CreatedBy.Email));
            if (!String.IsNullOrEmpty(reporterName) && !String.IsNullOrEmpty(reporterEmail))
            {
                destinationAddresses.Add(new MailboxAddress(reporterName, reporterEmail));
            }
            return destinationAddresses;
        }
    }
}
