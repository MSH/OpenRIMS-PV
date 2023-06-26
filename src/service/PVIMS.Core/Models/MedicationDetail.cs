using System;
using PVIMS.Core.CustomAttributes;

namespace PVIMS.Core.Models
{
    public class MedicationDetail: ExtendableDetail
    {
        public string MedicationSource { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime? DateEnd { get; set; }

        public string Dose { get; set; }

        public string DoseFrequency { get; set; }
    }
}
