using System;
using PVIMS.Core.CustomAttributes;

namespace PVIMS.Core.Models
{
    public class LabTestDetail: ExtendableDetail
    {
        public string LabTestSource { get; set; }

        public DateTime TestDate { get; set; }

        public string TestResult { get; set; }
    }
}
