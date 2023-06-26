using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A dataset element representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class DatasetElementDetailDto : DatasetElementIdentifierDto
    {
        /// <summary>
        /// E2B OID
        /// </summary>
        [DataMember]
        public string OID { get; set; }

        /// <summary>
        /// The default value for the element
        /// </summary>
        [DataMember]
        public string DefaultValue { get; set; }

        /// <summary>
        /// Is this a system defined element
        /// </summary>
        [DataMember]
        public string System { get; set; }

        /// <summary>
        /// Is this element mandatory
        /// </summary>
        [DataMember]
        public string Mandatory { get; set; }

        /// <summary>
        /// For string based elements, what is the maximum length of this element
        /// </summary>
        [DataMember]
        public int? MaxLength { get; set; }

        /// <summary>
        /// For string based elements, apply the following regular expression to this element
        /// </summary>
        [DataMember]
        public string RegEx { get; set; }

        /// <summary>
        /// For numeric based elements, how many decimals should be allowed
        /// </summary>
        [DataMember]
        public short? Decimals { get; set; }

        /// <summary>
        /// For numeric based elements, the upper range for the element
        /// </summary>
        [DataMember]
        public decimal? MaxSize { get; set; }

        /// <summary>
        /// For numeric based elements, the lower range for the element
        /// </summary>
        [DataMember]
        public decimal? MinSize { get; set; }

        /// <summary>
        /// Generate a calculated value for this element
        /// </summary>
        [DataMember]
        public string Calculation { get; set; }

        /// <summary>
        /// Should this element be anonymised in any form of data extraction
        /// </summary>
        [DataMember]
        public string Anonymise { get; set; }
    }
}
