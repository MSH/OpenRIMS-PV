using AutoMapper;
using LinqKit;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Models;
using PVIMS.Core.Aggregates.NotificationAggregate;
using PVIMS.Core.Repositories;
using Extensions = PVIMS.Core.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.NotificationAggregate
{
    public class NotificationsQueryHandler
        : IRequestHandler<NotificationsQuery, IEnumerable<NotificationDto>>
    {
        private readonly IRepositoryInt<Notification> _notificationRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationsQueryHandler> _logger;

        public NotificationsQueryHandler(
            IRepositoryInt<Notification> notificationRepository,
            IMapper mapper,
            ILogger<NotificationsQueryHandler> logger)
        {
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<NotificationDto>> Handle(NotificationsQuery message, CancellationToken cancellationToken)
        {
            var orderby = Extensions.GetOrderBy<Notification>("Created", "desc");

            var predicate = PredicateBuilder.New<Notification>(true);
            predicate = predicate.And(n => n.DestinationUserId == message.UserId);
            predicate = predicate.And(n => n.ValidUntilDate > DateTime.Now);

            var notificationsFromRepo = await _notificationRepository.ListAsync(predicate, orderby, new string[] { "" });
            if (notificationsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedNotifications = _mapper.Map<ICollection<NotificationDto>>(notificationsFromRepo);

                return mappedNotifications;
            }

            return null;
        }
    }
}