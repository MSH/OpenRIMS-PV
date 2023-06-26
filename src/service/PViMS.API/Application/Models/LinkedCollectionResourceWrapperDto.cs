using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    [DataContract()]
    public class LinkedCollectionResourceWrapperDto<T> : LinkedResourceBaseDto
        where T : LinkedResourceBaseDto
    {
        [DataMember()]
        public IEnumerable<T> Value { get; private set; }

        [DataMember()]
        public int RecordCount { get; private set; }

        [DataMember()]
        public int PageCount { get; private set; }

        // Include parameterless constructor to allow application/xml response (else 406 not accepted error returned)
        public LinkedCollectionResourceWrapperDto()
        {
        }

        public LinkedCollectionResourceWrapperDto(int totalRecordCount, IEnumerable<T> value, int totalPageCount = 0)
        {
            Value = value;
            RecordCount = totalRecordCount;
            PageCount = totalPageCount;
        }
    }
}
