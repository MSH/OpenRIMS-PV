using PVIMS.Core.SeedWork;
using System;

namespace PVIMS.Core.Models
{
    public class MetaElementSetList : Entity<int>
    {
        public string Element { get; set; }
        public Int64 Value { get; set; }
    }
}
