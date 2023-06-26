using MediatR;
using PVIMS.API.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.PatientAggregate
{
    [DataContract]
    public class AddMedicationToPatientCommand
        : IRequest<PatientMedicationIdentifierDto>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public string SourceDescription { get; private set; }

        [DataMember]
        public int ConceptId { get; private set; }

        [DataMember]
        public int? ProductId { get; private set; }

        [DataMember]
        public DateTime StartDate { get; private set; }

        [DataMember]
        public DateTime? EndDate { get; private set; }

        [DataMember]
        public string Dose { get; private set; }

        [DataMember]
        public string DoseFrequency { get; private set; }

        [DataMember]
        public string DoseUnit { get; private set; }

        [DataMember]
        public IDictionary<int, string> Attributes { get; private set; }

        public AddMedicationToPatientCommand()
        {
        }

        public AddMedicationToPatientCommand(int patientId, string sourceDescription, int conceptId, int? productId, DateTime startDate, DateTime? endDate, string dose, string doseFrequency, string doseUnit, IDictionary<int, string> attributes) : this()
        {
            PatientId = patientId;
            SourceDescription = sourceDescription;
            ConceptId = conceptId;
            ProductId = productId;
            StartDate = startDate;
            EndDate = endDate;
            Dose = dose;
            DoseFrequency = doseFrequency;
            DoseUnit = doseUnit;
            Attributes = attributes;
        }
    }
}
