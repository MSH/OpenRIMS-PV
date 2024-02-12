using OpenRIMS.PV.Main.API.Infrastructure.Attributes;
using OpenRIMS.PV.Main.Core.ValueTypes;

namespace OpenRIMS.PV.Main.API.Models.Parameters
{
    public class PatientTreatmentReportResourceParameters : BaseReportResourceParameters
    {
        /// <summary>
        /// Filter reports by criteria
        /// </summary>
        [ValidEnumValue]
        public PatientOnStudyCriteria PatientOnStudyCriteria { get; set; } = PatientOnStudyCriteria.HasEncounterinDateRange;
    }
}
