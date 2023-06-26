namespace PVIMS.Core.Entities
{
    public class RiskFactorOption : EntityBase
    {
        public string OptionName { get; set; }
        public string Criteria { get; set; }
        public string Display { get; set; }
        public int RiskFactorId { get; set; }

        public virtual RiskFactor RiskFactor { get; set; }
    }
}
