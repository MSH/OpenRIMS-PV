using MediatR;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.AttachmentAggregate
{
    [DataContract]
    public class ArchiveAttachmentCommand
        : IRequest<bool>
    {
        [DataMember]
        public int? PatientId { get; private set; }

        [DataMember]
        public int AttachmentId { get; private set; }

        [DataMember]
        public string Reason { get; private set; }


        public ArchiveAttachmentCommand()
        {
        }

        public ArchiveAttachmentCommand(int patientId, int attachmentId, string reason) : this()
        {
            PatientId = patientId;
            AttachmentId = attachmentId;
            Reason = reason;
        }
    }
}
