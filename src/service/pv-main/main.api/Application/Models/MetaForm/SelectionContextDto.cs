using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models.MetaForm
{
    /// <summary>
    /// The definition of the element
    /// </summary>
    [DataContract()]
    public class SelectionContextDto
    {
        /// <summary>
        /// A list of selection values associated to the attribute (if applicable)
        /// </summary>
        [DataMember]
        public ICollection<SelectionDataItemDto> SelectionDataItems { get; set; } = new List<SelectionDataItemDto>();

        /// <summary>
        /// Use a service to collect the data
        /// </summary>
        [DataMember]
        public bool UseService { get; set; }

        /// <summary>
        /// The service to be used to populate selection values
        /// </summary>
        [DataMember]
        public string ServiceDefinition { get; set; }

    }
}
