using MediatR;
using Microsoft.AspNetCore.Http;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.AttachmentAggregate
{
    [DataContract]
    public class AddAttachmentCommand
        : IRequest<AttachmentIdentifierDto>
    {
        [DataMember]
        public int? PatientId { get; private set; }

        [DataMember]
        public string Description { get; private set; }

        [DataMember]
        public IFormFile Attachment { get; private set; }

        public AddAttachmentCommand()
        {
        }

        public AddAttachmentCommand(int? patientId, string description, IFormFile attachment) : this()
        {
            PatientId = patientId;
            Description = description;
            Attachment = attachment;
        }
    }
}
