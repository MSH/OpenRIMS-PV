using MediatR;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.PatientAggregate
{
    [DataContract]
    public class ChangeMedicationDetailsCommand
        : IRequest<bool>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public int PatientMedicationId { get; private set; }

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

        public ChangeMedicationDetailsCommand()
        {
        }

        public ChangeMedicationDetailsCommand(int patientId, int patientMedicationId, DateTime startDate, DateTime? endDate, string dose, string doseFrequency, string doseUnit, IDictionary<int, string> attributes) : this()
        {
            PatientId = patientId;
            PatientMedicationId = patientMedicationId;
            StartDate = startDate;
            EndDate = endDate;
            Dose = dose;
            DoseFrequency = doseFrequency;
            DoseUnit = doseUnit;
            Attributes = attributes;
        }
    }
}
