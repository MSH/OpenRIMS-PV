using System.Runtime.Serialization;

namespace PVIMS.API.Models.MetaForm
{
    /// <summary>
    /// A meta form element representation containing a list of elements available in a form category
    /// </summary>
    [DataContract()]
    public class ElementDto
    {
        /// <summary>
        /// Type the element is associated to
        /// </summary>
        [DataMember]
        public string ContextType { get; set; }

        /// <summary>
        /// The display name for the element
        /// </summary>
        [DataMember]
        public string ElementName { get; set; }

        /// <summary>
        /// COntextual definition of the element
        /// </summary>
        [DataMember]
        public ContextDefinitionDto ContextDefinition { get; set; }
    }
}
