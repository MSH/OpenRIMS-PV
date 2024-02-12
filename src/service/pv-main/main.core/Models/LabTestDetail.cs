using System;
using OpenRIMS.PV.Main.Core.CustomAttributes;

namespace OpenRIMS.PV.Main.Core.Models
{
    public class LabTestDetail: ExtendableDetail
    {
        public string LabTestSource { get; set; }

        public DateTime TestDate { get; set; }

        public string TestResult { get; set; }
    }
}
