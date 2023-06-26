using MediatR;
using MimeKit;
using PViMS.Core.Events;
using PVIMS.API.Infrastructure.Services;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.DomainEventHandlers.TaskAdded
{
    public class SendEmailWhenCausalityConfirmedDomainEventHandler
                            : INotificationHandler<CausalityConfirmedDomainEvent>
    {
        private readonly ISMTPMailService _smtpMailService;
        private readonly ICustomAttributeService _attributeService;
        private readonly IRepositoryInt<PatientClinicalEvent> _patientClinicalEventRepository;

        public SendEmailWhenCausalityConfirmedDomainEventHandler(ISMTPMailService smtpMailService,
            ICustomAttributeService attributeService,
            IRepositoryInt<PatientClinicalEvent> patientClinicalEventRepository)
        {
            _smtpMailService = smtpMailService ?? throw new ArgumentNullException(nameof(smtpMailService));
            _attributeService = attributeService ?? throw new ArgumentNullException(nameof(attributeService));
            _patientClinicalEventRepository = patientClinicalEventRepository ?? throw new ArgumentNullException(nameof(patientClinicalEventRepository));
        }

        public async Task Handle(CausalityConfirmedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var subject = $"Report: {domainEvent.ReportInstance.PatientIdentifier}";

            var sb = new StringBuilder();
            sb.Append($"Causality and terminology has been set for this report. Please note the following details pertaining to the report: ");
            sb.Append("<p><b><u>Adverse Event Details</u></b></p>");
            sb.Append("<table>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Identifier</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.ReportInstance.Identifier}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Patient</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.ReportInstance.PatientIdentifier}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Created</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.ReportInstance.Created}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Description</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.ReportInstance.SourceIdentifier}</td></tr>");
            sb.Append("</table>");
            sb.Append("<p><b><u>Causality and Terminology Details</u></b></p>");
            sb.Append("<table>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Terminology</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.ReportInstance.TerminologyMedDra.DisplayName}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Classification</b></td><td style='padding: 10px; border: 1px solid black;'>{ReportClassification.From(domainEvent.ReportInstance.ReportClassificationId)}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Causality</b></td><td style='padding: 10px; border: 1px solid black;'>Please browse PV feedback for more details</td></tr>");
            sb.Append("</table>");
            sb.Append("<p><b>*** This is system generated. Please do not reply to this message ***</b></p>");

            await _smtpMailService.SendEmailAsync(subject, sb.ToString(), await PrepareDestinationMailBoxesAsync(domainEvent));
        }

        private async Task<List<MailboxAddress>> PrepareDestinationMailBoxesAsync(CausalityConfirmedDomainEvent domainEvent)
        {
            var patientClinicalEvent = await _patientClinicalEventRepository.GetAsync(pce => pce.PatientClinicalEventGuid == domainEvent.ReportInstance.ContextGuid,
                new string[] { });
            if (patientClinicalEvent == null)
            {
                throw new KeyNotFoundException(nameof(patientClinicalEvent));
            }

            var extendable = (IExtendable)patientClinicalEvent;
            var reporterName = await _attributeService.GetCustomAttributeValueAsync("PatientClinicalEvent", "Name of reporter", extendable);
            var reporterEmail = await _attributeService.GetCustomAttributeValueAsync("PatientClinicalEvent", "Email address", extendable);

            var destinationAddresses = new List<MailboxAddress>();
            destinationAddresses.Add(new MailboxAddress(domainEvent.ReportInstance.CreatedBy.FullName, domainEvent.ReportInstance.CreatedBy.Email));
            if (!String.IsNullOrEmpty(reporterName) && !String.IsNullOrEmpty(reporterEmail))
            {
                destinationAddresses.Add(new MailboxAddress(reporterName, reporterEmail));
            }
            return destinationAddresses;
        }
    }
}
