using OpenRIMS.PV.Main.Core.SeedWork;
using System;

namespace OpenRIMS.PV.Main.Core.Models
{
    public class MetaElementSetList : Entity<int>
    {
        public string Element { get; set; }
        public Int64 Value { get; set; }
    }
}
