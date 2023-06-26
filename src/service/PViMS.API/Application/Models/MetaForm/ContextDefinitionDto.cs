using System.Runtime.Serialization;

namespace PVIMS.API.Models.MetaForm
{
    /// <summary>
    /// The definition of the element
    /// </summary>
    [DataContract()]
    public class ContextDefinitionDto
    {
        /// <summary>
        /// The type of element  
        /// Valid options are string, selection, dateTime, numeric, table
        /// </summary>
        [DataMember]
        public string ElementType { get; set; }

        /// <summary>
        /// Is this element mandatory
        /// </summary>
        [DataMember]
        public bool Required { get; set; }

        /// <summary>
        /// Selection context for selection element type
        /// </summary>
        [DataMember]
        public SelectionContextDto SelectionContext { get; set; }
    }
}
