using MediatR;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.PatientAggregate
{
    [DataContract]
    public class ChangePatientNotesCommand
        : IRequest<bool>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public string Notes { get; set; }

        public ChangePatientNotesCommand()
        {
        }

        public ChangePatientNotesCommand(int patientId, string notes) : this()
        {
            PatientId = patientId;
            Notes = notes;
        }
    }
}