﻿using MediatR;
using MimeKit;
using OpenRIMS.PV.Main.Core.Events;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;
using OpenRIMS.PV.Main.Core.CustomAttributes;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.DomainEventHandlers.TaskAdded
{
    public class SendEmailWhenTaskAddedDomainEventHandler
                            : INotificationHandler<TaskAddedDomainEvent>
    {
        private readonly ISMTPMailService _smtpMailService;
        private readonly ICustomAttributeService _attributeService;
        private readonly IRepositoryInt<PatientClinicalEvent> _patientClinicalEventRepository;

        public SendEmailWhenTaskAddedDomainEventHandler(ISMTPMailService smtpMailService,
            ICustomAttributeService attributeService,
            IRepositoryInt<PatientClinicalEvent> patientClinicalEventRepository)
        {
            _smtpMailService = smtpMailService ?? throw new ArgumentNullException(nameof(smtpMailService));
            _attributeService = attributeService ?? throw new ArgumentNullException(nameof(attributeService));
            _patientClinicalEventRepository = patientClinicalEventRepository ?? throw new ArgumentNullException(nameof(patientClinicalEventRepository));
        }

        public async Task Handle(TaskAddedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            if (_smtpMailService.CheckIfEnabled())
            {
                var subject = $"Report task: {domainEvent.Task.ReportInstance.Identifier}";

                var sb = new StringBuilder();
                sb.Append($"A new task {TaskType.From(domainEvent.Task.TaskTypeId).Name} has been added that requires your attention. Please note the following details pertaining to the task: ");
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
        }

        private async Task<List<MailboxAddress>> PrepareDestinationMailBoxesAsync(TaskAddedDomainEvent domainEvent)
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