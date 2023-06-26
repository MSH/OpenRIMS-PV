using System.Linq;

namespace PVIMS.Core.Entities
{
    public class ConditionMedDra : EntityBase
	{
        public int ConditionId { get; set; }
        public int TerminologyMedDraId { get; set; }

        public virtual Condition Condition { get; set; }
        public virtual TerminologyMedDra TerminologyMedDra { get; set; }

        public PatientCondition GetConditionForPatient(Patient patient)
        {
            return patient.PatientConditions.OrderByDescending(pc => pc.OnsetDate).Where(pc => TerminologyMedDra.Id == pc.TerminologyMedDra.Id && pc.OutcomeDate == null).FirstOrDefault();
        }

        public PatientCondition GetConditionForEncounter(Encounter encounter)
        {
            PatientCondition tempCondition = encounter.Patient.PatientConditions.OrderByDescending(pc => pc.OnsetDate).Where(pc => TerminologyMedDra.Id == pc.TerminologyMedDra.Id && ((pc.OutcomeDate == null && pc.OnsetDate <= encounter.EncounterDate) || (pc.OutcomeDate >= encounter.EncounterDate && pc.OnsetDate <= encounter.EncounterDate))).FirstOrDefault();
            return tempCondition;
        }

	}
}