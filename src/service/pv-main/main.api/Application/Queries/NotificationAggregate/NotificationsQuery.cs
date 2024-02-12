using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.NotificationAggregate
{
    [DataContract]
    public class NotificationsQuery
        : IRequest<IEnumerable<NotificationDto>>
    {
        [DataMember]
        public int UserId { get; private set; }

        public NotificationsQuery()
        {
        }

        public NotificationsQuery(int userId) : this()
        {
            UserId = userId;
        }
    }
}
