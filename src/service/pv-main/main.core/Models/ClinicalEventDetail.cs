using System;
using OpenRIMS.PV.Main.Core.CustomAttributes;

namespace OpenRIMS.PV.Main.Core.Models
{
    public class ClinicalEventDetail: ExtendableDetail
    {
        public string SourceDescription { get; set; }

        public int? SourceTerminologyMedDraId { get; set; }

        public DateTime? OnsetDate { get; set; }

        public DateTime? ResolutionDate { get; set; }
    }
}
