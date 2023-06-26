using PVIMS.Core.Aggregates.DatasetAggregate;
using System;
using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
    public class DatasetInstanceValue : EntityBase
    {
        protected DatasetInstanceValue()
        {
            DatasetInstanceSubValues = new HashSet<DatasetInstanceSubValue>();
        }

        public DatasetInstanceValue(DatasetElement datasetElement, DatasetInstance datasetInstance, string instanceValue)
            :this()
        {
            this.DatasetElement = datasetElement;
            this.InstanceValue = instanceValue;
        }

        public string InstanceValue { get; set; }
        public int DatasetElementId { get; set; }
        public int DatasetInstanceId { get; set; }

        public virtual DatasetElement DatasetElement { get; set; }
        public virtual DatasetInstance DatasetInstance { get; set; }

        public virtual ICollection<DatasetInstanceSubValue> DatasetInstanceSubValues { get; set; }

        public DatasetInstanceSubValue AddDatasetInstanceSubValue(DatasetElementSub datasetElementSub, string instanceSubValue, Guid contextValue = default(Guid))
        {
            var datasetInstanceSubValue = new DatasetInstanceSubValue(datasetElementSub, this, instanceSubValue, contextValue);

            DatasetInstanceSubValues.Add(datasetInstanceSubValue);

            return datasetInstanceSubValue;
        }
    }
}