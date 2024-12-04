using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate
{
    public class ReportInstanceCausality
        : EntityBase
    {
        public int ReportInstanceId { get; private set; }
        public virtual ReportInstance ReportInstance { get; private set; }

        protected ReportInstanceCausality()
        {
        }
   }
}