using MediatR;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.PatientAggregate
{
    [DataContract]
    public class ChangePatientFacilityCommand
        : IRequest<bool>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public string FacilityName { get; private set; }

        public ChangePatientFacilityCommand()
        {
        }

        public ChangePatientFacilityCommand(int patientId, string facilityName) : this()
        {
            PatientId = patientId;
            FacilityName = facilityName;
        }
    }
}