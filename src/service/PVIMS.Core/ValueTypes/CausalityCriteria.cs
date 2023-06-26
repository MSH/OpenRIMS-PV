using System.ComponentModel;

namespace PVIMS.Core.ValueTypes
{
    public enum CausalityCriteria
    {
        [Description("Causality Set")]
        CausalitySet = 1,

        [Description("Causality Not Set")]
        CausalityNotSet = 2
    }
}
