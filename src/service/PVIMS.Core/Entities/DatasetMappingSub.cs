using PVIMS.Core.ValueTypes;
using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
    public class DatasetMappingSub : EntityBase
    {
        public DatasetMappingSub()
        {
            DatasetMappingValues = new HashSet<DatasetMappingValue>();
        }

        public string PropertyPath { get; set; }
        public string Property { get; set; }
        public MappingType MappingType { get; set; }
        public string MappingOption { get; set; }
        public int DestinationElementId { get; set; }
        public int MappingId { get; set; }
        public int? SourceElementId { get; set; }

        public virtual DatasetMapping Mapping { get; set; }
        public virtual DatasetElementSub DestinationElement { get; set; }
        public virtual DatasetElementSub SourceElement { get; set; }

        public virtual ICollection<DatasetMappingValue> DatasetMappingValues { get; set; }
    }
}