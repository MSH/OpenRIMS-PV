using System;

namespace PVIMS.API.Models.Parameters
{
    public class OutstandingVisitResourceParameters : BaseReportResourceParameters
    {
        /// <summary>
        /// Filter appointments by facility id
        /// </summary>
        public int FacilityId { get; set; } = 0;
    }
}
