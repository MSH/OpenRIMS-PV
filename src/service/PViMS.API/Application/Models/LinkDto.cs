using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    [DataContract()]
    public class LinkDto
    {
        /// <summary>
        /// The URI that will need to be referenced to invoke the RESTful API
        /// </summary>
        [DataMember]
        public string Href { get; private set; }

        /// <summary>
        /// The relative name that uniquely identifies the action to the consumer
        /// </summary>
        [DataMember]
        public string Rel { get; private set; }

        /// <summary>
        /// The HTTP verb that will be invoked through this action
        /// </summary>
        [DataMember]
        public string Method { get; private set; }

        public LinkDto(string href, string rel, string method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }
    }
}
