using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A patient lab test representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class PatientLabTestDetailDto : PatientLabTestIdentifierDto
    {
        /// <summary>
        /// The name of the lab test
        /// </summary>
        [DataMember]
        public string LabTest { get; set; }

        /// <summary>
        /// The date of the test
        /// </summary>
        [DataMember]
        public string TestDate { get; set; }

        /// <summary>
        /// Test result - coded
        /// </summary>
        [DataMember]
        public string TestResultCoded { get; set; }

        /// <summary>
        /// Test result - value
        /// </summary>
        [DataMember]
        public string TestResultValue { get; set; }

        /// <summary>
        /// The unit of the test
        /// </summary>
        [DataMember]
        public string TestUnit { get; set; }

        /// <summary>
        /// The lower range of the test result
        /// </summary>
        [DataMember]
        public string ReferenceLower { get; set; }

        /// <summary>
        /// The upper range of the test result
        /// </summary>
        [DataMember]
        public string ReferenceUpper { get; set; }

        /// <summary>
        /// A list of custom attributes associated to the patient condition
        /// </summary>
        [DataMember]
        public ICollection<AttributeValueDto> LabTestAttributes { get; set; } = new List<AttributeValueDto>();
    }
}
