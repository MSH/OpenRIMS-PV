using System;

namespace OpenRIMS.PV.Main.Core.Entities
{
    public class DatasetInstanceSubValue : EntityBase
    {
        protected DatasetInstanceSubValue() { }

        public DatasetInstanceSubValue(DatasetElementSub datasetElementSub, DatasetInstanceValue datasetInstanceValue, string instanceSubValue)
            : this(datasetElementSub, datasetInstanceValue, instanceSubValue, default(Guid))
        {
        }

        public DatasetInstanceSubValue(DatasetElementSub datasetElementSub, DatasetInstanceValue datasetInstanceValue, string instanceSubValue, Guid contextValue)
        {
            if (contextValue == default(Guid))
            {
                ContextValue = Guid.NewGuid();
            }
            else
            {
                ContextValue = contextValue;
            }

            this.DatasetElementSub = datasetElementSub;
            this.DatasetInstanceValue = datasetInstanceValue;
            this.InstanceValue = instanceSubValue;
        }

        public Guid ContextValue { get; set; }
        public string InstanceValue { get; set; }
        public int DatasetElementSubId { get; set; }
        public int DatasetInstanceValueId { get; set; }

        public virtual DatasetInstanceValue DatasetInstanceValue { get; set; }
        public virtual DatasetElementSub DatasetElementSub { get; set; }
    }
}