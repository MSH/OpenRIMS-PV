﻿using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.CohortGroupAggregate
{
    [DataContract]
    public class CohortGroupsIdentifierQuery
        : IRequest<LinkedCollectionResourceWrapperDto<CohortGroupIdentifierDto>>
    {
        [DataMember]
        public string OrderBy { get; private set; }

        [DataMember]
        public int PageNumber { get; private set; }

        [DataMember]
        public int PageSize { get; private set; }

        public CohortGroupsIdentifierQuery()
        {
        }

        public CohortGroupsIdentifierQuery(string orderBy, int pageNumber, int pageSize) : this()
        {
            OrderBy = orderBy;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
