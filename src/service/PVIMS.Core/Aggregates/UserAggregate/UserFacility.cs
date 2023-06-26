using PVIMS.Core.Entities;

namespace PVIMS.Core.Aggregates.UserAggregate
{
    public class UserFacility : EntityBase
    {
        public int FacilityId { get; private set; }
        public Facility Facility { get; private set; }

        public int UserId { get; private set; }
        public User User { get; private set; }

        protected UserFacility()
        {

        }

        public UserFacility(Facility facility, User user): this()
        {
            FacilityId = facility.Id;
            Facility = facility;
            UserId = user.Id;
            User = user;
        }
    }
}
