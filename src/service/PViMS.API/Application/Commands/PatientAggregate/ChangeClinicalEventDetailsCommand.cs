using MediatR;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.PatientAggregate
{
    [DataContract]
    public class ChangeClinicalEventDetailsCommand
        : IRequest<bool>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public int PatientClinicalEventId { get; private set; }

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

        public ChangeClinicalEventDetailsCommand()
        {
        }

        public ChangeClinicalEventDetailsCommand(int patientId, int patientClinicalEventId, string sourceDescription, int? sourceTerminologyMedDraId, DateTime onsetDate, DateTime? resolutionDate, IDictionary<int, string> attributes) : this()
        {
            PatientId = patientId;
            PatientClinicalEventId = patientClinicalEventId;
            SourceDescription = sourceDescription;
            SourceTerminologyMedDraId = sourceTerminologyMedDraId;
            OnsetDate = onsetDate;
            ResolutionDate = resolutionDate;
            Attributes = attributes;
        }
    }
}
