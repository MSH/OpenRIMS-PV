using System;

namespace PVIMS.Core.Entities
{
    public class Holiday : EntityBase
    {
        public DateTime HolidayDate { get; set; }
        public string Description { get; set; }
    }
}
