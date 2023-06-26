using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
    public class RiskFactor : EntityBase
    {
        public RiskFactor()
        {
            IsSystem = false;
            Active = true;

            Options = new HashSet<RiskFactorOption>();
        }

        public string FactorName { get; set; }
        public string Criteria { get; set; }
        public string Display { get; set; }
        public bool IsSystem { get; set; }
        public bool Active { get; set; }

        public virtual ICollection<RiskFactorOption> Options { get; set; }
    }
}