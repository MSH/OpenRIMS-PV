﻿namespace OpenRIMS.PV.Main.Core.Paging
{
    public interface IPagingInfo
    {
        int PageNumber { get; set; }
        int PageSize { get; set; }
    }
}
