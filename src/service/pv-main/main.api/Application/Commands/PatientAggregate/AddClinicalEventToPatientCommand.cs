using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.PatientAggregate
{
    [DataContract]
    public class AddClinicalEventToPatientCommand
        : IRequest<PatientClinicalEventIdentifierDto>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public string PatientIdentifier { get; private set; }

        [DataMember]
        public string SourceDescription { get; private set; }

        [DataMember]
        public int? SourceTerminologyMedDraId { get; private set; }

        [DataMember]
        public DateTime OnsetDate { get; private set; }

        [DataMember]
        public DateTime? ResolutionDate { get; private set; }

        [DataMember]
        public IDictionary<int, string> Attributes { get; private set; }

        public AddClinicalEventToPatientCommand()
        {
        }

        public AddClinicalEventToPatientCommand(int patientId, string patientIdentifier, string sourceDescription, int? sourceTerminologyMedDraId, DateTime onsetDate, DateTime? resolutionDate, IDictionary<int, string> attributes) : this()
        {
            PatientId = patientId;
            PatientIdentifier = patientIdentifier;
            SourceDescription = sourceDescription;
            SourceTerminologyMedDraId = sourceTerminologyMedDraId;
            OnsetDate = onsetDate;
            ResolutionDate = resolutionDate;
            Attributes = attributes;
        }
    }
}
