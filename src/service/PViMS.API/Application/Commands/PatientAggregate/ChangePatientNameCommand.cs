using MediatR;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.PatientAggregate
{
    [DataContract]
    public class ChangePatientNameCommand
        : IRequest<bool>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string MiddleName { get; set; }

        public ChangePatientNameCommand()
        {
        }

        public ChangePatientNameCommand(int patientId, string firstName, string middleName, string lastName) : this()
        {
            PatientId = patientId;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
        }
    }
}