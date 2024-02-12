using System;
using System.Collections.Generic;

namespace OpenRIMS.PV.Main.API.Models
{
    public class PatientLabTestForUpdateDto
    {
        /// <summary>
        /// The look up value for the lab test
        /// </summary>
        public string LabTest { get; set; }

        /// <summary>
        /// The date the test was conducted
        /// </summary>
        public DateTime TestDate { get; set; }

        /// <summary>
        /// Test result - coded
        /// </summary>
        public string TestResultCoded { get; set; }

        /// <summary>
        /// Test result - value
        /// </summary>
        public string TestResultValue { get; set; }

        /// <summary>
        /// The look up value for the test unit
        /// </summary>
        public string TestUnit { get; set; }

        /// <summary>
        /// The lower range of the test result
        /// </summary>
        public string ReferenceLower { get; set; }

        /// <summary>
        /// The upper range of the test result
        /// </summary>
        public string ReferenceUpper { get; set; }

        /// <summary>
        /// Lab test custom attributes
        /// </summary>
        public ICollection<AttributeValueForPostDto> Attributes { get; set; }
    }
}