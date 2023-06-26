using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.Core.Models
{
    public class SeriesValueList
    {
        public SeriesValueList()
        {
            Series = new HashSet<SeriesValueListItem>();
        }

        public string Name { get; set; }

        public virtual ICollection<SeriesValueListItem> Series { get; set; }
    }

    public class SeriesValueListItem
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Value { get; set; }

        //[Required]
        //public int Min { get; set; }

        //[Required]
        //public int Max { get; set; }
    }

}
