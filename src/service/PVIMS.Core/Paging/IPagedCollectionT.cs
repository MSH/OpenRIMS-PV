using System.Collections.Generic;

namespace PVIMS.Core.Paging
{
    public interface IPagedCollection<T>
    {
        int TotalRowCount { get; set; }
        ICollection<T> Data { get; set; }
    }
}
