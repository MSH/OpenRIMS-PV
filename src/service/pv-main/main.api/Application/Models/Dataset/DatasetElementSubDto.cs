using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A dataset element sub representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class DatasetElementSubDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the dataset element sub
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The name of the dataset element sub
        /// </summary>
        [DataMember]
        public string ElementName { get; set; }

        /// <summary>
        /// The type of field
        /// </summary>
        [DataMember]
        public string FieldTypeName { get; set; }

        /// <summary>
        /// the order of the field within the dataset element
        /// </summary>
        [DataMember]
        public short FieldOrder { get; set; }

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
        /// A friendly description of the element sub
        /// </summary>
        [DataMember]
        public string FriendlyName { get; set; }

        /// <summary>
        /// Additional help for the element sub
        /// </summary>
        [DataMember]
        public string Help { get; set; }

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
