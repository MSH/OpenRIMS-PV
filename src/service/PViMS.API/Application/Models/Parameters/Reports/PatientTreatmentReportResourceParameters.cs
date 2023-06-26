using PVIMS.API.Infrastructure.Attributes;
using PVIMS.Core.ValueTypes;

namespace PVIMS.API.Models.Parameters
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
