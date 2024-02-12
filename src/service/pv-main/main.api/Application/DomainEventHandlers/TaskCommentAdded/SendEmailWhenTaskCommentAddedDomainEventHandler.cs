using MediatR;
using MimeKit;
using OpenRIMS.PV.Main.Core.Events;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.DomainEventHandlers.TaskCommentAdded
{
    public class SendEmailWhenTaskCommentAddedDomainEventHandler
                            : INotificationHandler<TaskCommentAddedDomainEvent>
    {
        private readonly ISMTPMailService _smtpMailService;

        public SendEmailWhenTaskCommentAddedDomainEventHandler(ISMTPMailService smtpMailService)
        {
            _smtpMailService = smtpMailService ?? throw new ArgumentNullException(nameof(smtpMailService));
        }

        public async Task Handle(TaskCommentAddedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            if (_smtpMailService.CheckIfEnabled())
            {
                var subject = $"Report task: {domainEvent.Comment.ReportInstanceTask.ReportInstance.Identifier}";

                var sb = new StringBuilder();
                sb.Append($"A new comment has been added for task {domainEvent.Comment.ReportInstanceTask.ReportInstance.Identifier}: ");
                sb.Append("<p><b><u>Comment</u></b></p>");
                sb.Append("<table>");
                sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Comment</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.Comment.Comment}</td></tr>");
                sb.Append("</table>");
                sb.Append("<p><b><u>Adverse Event Details</u></b></p>");
                sb.Append("<table>");
                sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Identifier</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.Comment.ReportInstanceTask.ReportInstance.Identifier}</td></tr>");
                sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Patient</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.Comment.ReportInstanceTask.ReportInstance.PatientIdentifier}</td></tr>");
                sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Created</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.Comment.ReportInstanceTask.ReportInstance.Created}</td></tr>");
                sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Description</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.Comment.ReportInstanceTask.ReportInstance.SourceIdentifier}</td></tr>");
                sb.Append("</table>");
                sb.Append("<p><b><u>Task Details</u></b></p>");
                sb.Append("<table>");
                sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Source</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.Comment.ReportInstanceTask.TaskDetail.Source}</td></tr>");
                sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Description</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.Comment.ReportInstanceTask.TaskDetail.Description}</td></tr>");
                sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Status</b></td><td style='padding: 10px; border: 1px solid black;'>{Core.Aggregates.ReportInstanceAggregate.TaskStatus.From(domainEvent.Comment.ReportInstanceTask.TaskStatusId).Name}</td></tr>");
                sb.Append("</table>");
                sb.Append("<p><b>*** This is system generated. Please do not reply to this message ***</b></p>");

                var destinationUser = GetDestinationUser(domainEvent);

                await _smtpMailService.SendEmailAsync(subject, sb.ToString(), PrepareDestinationMailBoxes(destinationUser));
            }
        }

        private User GetDestinationUser(TaskCommentAddedDomainEvent domainEvent)
        {
            User destinationUser;
            if (domainEvent.Comment.CreatedById == domainEvent.Comment.ReportInstanceTask.ReportInstance.CreatedById)
            {
                // comment created by clinician, so send to person who created task
                destinationUser = domainEvent.Comment.ReportInstanceTask.CreatedBy;
            }
            else
            {
                // comment created by analyst, so send to person who created report
                destinationUser = domainEvent.Comment.ReportInstanceTask.ReportInstance.CreatedBy;
            }

            return destinationUser;
        }

        private List<MailboxAddress> PrepareDestinationMailBoxes(User destinationUser)
        {
            var destinationAddresses = new List<MailboxAddress>
            {
                new MailboxAddress(destinationUser.FullName, destinationUser.Email)
            };

            return destinationAddresses;
        }
    }
}
