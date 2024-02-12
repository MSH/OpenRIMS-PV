using OpenRIMS.PV.Main.Core.Entities;
using System;

namespace OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate
{
    public class ReportInstanceMedication 
        : EntityBase
    {
        public string MedicationIdentifier { get; private set; }
        public string NaranjoCausality { get; private set; }
        public string WhoCausality { get; private set; }
        public Guid ReportInstanceMedicationGuid { get; set; }
        public int ReportInstanceId { get; private set; }

        public virtual ReportInstance ReportInstance { get; private set; }

        protected ReportInstanceMedication()
        {
        }

        public ReportInstanceMedication(string medicationIdentifier, string naranjoCausality, string whoCausality, Guid reportInstanceMedicationGuid)
        {
            MedicationIdentifier = medicationIdentifier;
            NaranjoCausality = naranjoCausality;
            WhoCausality = whoCausality;
            ReportInstanceMedicationGuid = reportInstanceMedicationGuid;
        }

        public void ChangeMedicationIdentifier(string medicationIdentifier)
        {
            MedicationIdentifier = medicationIdentifier;
        }

        public void ChangeWhoCausality(string whoCausality)
        {
            WhoCausality = whoCausality;
        }

        public void ChangeNaranjoCausality(string naranjoCausality)
        {
            NaranjoCausality = naranjoCausality;
        }
    }
}