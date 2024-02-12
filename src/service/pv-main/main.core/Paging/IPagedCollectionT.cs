using System.Collections.Generic;

namespace OpenRIMS.PV.Main.Core.Paging
{
    public interface IPagedCollection<T>
    {
        int TotalRowCount { get; set; }
        ICollection<T> Data { get; set; }
    }
}
