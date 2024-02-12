using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    [DataContract()]
    public abstract class LinkedResourceBaseDto
    {
        [DataMember()]
        public List<LinkDto> Links { get; set; }
                = new List<LinkDto>();
    }
}
