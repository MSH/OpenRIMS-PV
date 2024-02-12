using System.Collections.Generic;

namespace OpenRIMS.PV.Main.Core.Entities
{
    public class MedDRAScale : EntityBase
	{
        public MedDRAScale()
        {
            Grades = new HashSet<MedDRAGrading>();
        }

        public int GradingScaleId { get; set; }
        public int TerminologyMedDraId { get; set; }

        public virtual TerminologyMedDra TerminologyMedDra { get; set; }
        public virtual SelectionDataItem GradingScale { get; set; }

        public virtual ICollection<MedDRAGrading> Grades { get; set; }
	}
}