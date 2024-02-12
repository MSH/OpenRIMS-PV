using System.ComponentModel;

namespace OpenRIMS.PV.Main.Core.ValueTypes
{
    public enum CausalityConfigType
    {
        [Description("Both Scales")]
        BothScales = 1,
        [Description("Naranjo Scale")]
        NaranjoScale = 2,
        [Description("WHO Scale")]
        WHOScale = 3
    }
}
