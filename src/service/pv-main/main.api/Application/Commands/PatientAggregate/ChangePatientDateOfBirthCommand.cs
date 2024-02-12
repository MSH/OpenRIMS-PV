using MediatR;
using System;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.PatientAggregate
{
    [DataContract]
    public class ChangePatientDateOfBirthCommand
        : IRequest<bool>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public DateTime DateOfBirth { get; set; }

        public ChangePatientDateOfBirthCommand()
        {
        }

        public ChangePatientDateOfBirthCommand(int patientId, DateTime dateOfBirth) : this()
        {
            PatientId = patientId;
            DateOfBirth = dateOfBirth;
        }
    }
}