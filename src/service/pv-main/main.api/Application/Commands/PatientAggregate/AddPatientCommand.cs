using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.PatientAggregate
{
    [DataContract]
    public class AddPatientCommand
        : IRequest<PatientIdentifierDto>
    {
        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string MiddleName { get; set; }

        [DataMember]
        public DateTime DateOfBirth { get; set; }

        [DataMember]
        public string FacilityName { get; set; }

        [DataMember]
        public int ConditionGroupId { get; set; }

        [DataMember]
        public int MeddraTermId { get; set; }

        [DataMember]
        public int? CohortGroupId { get; set; }

        [DataMember]
        public DateTime? EnroledDate { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public DateTime? OutcomeDate { get; set; }

        [DataMember]
        public string Comments { get; set; }

        [DataMember]
        public string CaseNumber { get; set; }

        [DataMember]
        public int EncounterTypeId { get; set; }

        [DataMember]
        public int PriorityId { get; set; }

        [DataMember]
        public DateTime EncounterDate { get; set; }

        [DataMember]
        public IDictionary<int, string> Attributes { get; private set; }

        public AddPatientCommand(string firstName, string lastName, string middleName, DateTime dateOfBirth, string facilityName, int conditionGroupId, int meddraTermId, int? cohortGroupId, DateTime? enroledDate, DateTime startDate, DateTime? outcomeDate, string caseNumber, string comments, int encounterTypeId, int priorityId, DateTime encounterDate, IDictionary<int, string> attributes)
        {
            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;
            DateOfBirth = dateOfBirth;
            FacilityName = facilityName;
            ConditionGroupId = conditionGroupId;
            MeddraTermId = meddraTermId;
            CohortGroupId = cohortGroupId;
            EnroledDate = enroledDate;
            StartDate = startDate;
            OutcomeDate = outcomeDate;
            CaseNumber = caseNumber;
            Comments = comments;
            EncounterTypeId = encounterTypeId;
            PriorityId = priorityId;
            EncounterDate = encounterDate;
            Attributes = attributes;
        }
    }
}
