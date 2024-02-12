using System;

namespace OpenRIMS.PV.Main.API.Models.Parameters
{
    public class BaseReportResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Filter by range
        /// </summary>
        public DateTime SearchFrom { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Filter by range
        /// </summary>
        public DateTime SearchTo { get; set; } = DateTime.MaxValue;
    }
}
