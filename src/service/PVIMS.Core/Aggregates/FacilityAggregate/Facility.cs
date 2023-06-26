using PVIMS.Core.Aggregates.UserAggregate;
using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
    public class Facility : EntityBase
	{
        protected Facility()
        {
            PatientFacilities = new HashSet<PatientFacility>();
            UserFacilities = new HashSet<UserFacility>();
        }

        public Facility(string facilityName, string facilityCode, FacilityType facilityType, string telNumber, string mobileNumber, string faxNumber, OrgUnit orgUnit)
        {
            FacilityName = facilityName;
            FacilityCode = facilityCode;

            FacilityTypeId = facilityType.Id;
            FacilityType = facilityType;

            TelNumber = telNumber;
            MobileNumber = mobileNumber;
            FaxNumber = faxNumber;

            OrgUnitId = orgUnit?.Id;
            OrgUnit = orgUnit;
        }

        public string FacilityCode { get; private set; }
        public string FacilityName { get; private set; }
        
        public int FacilityTypeId { get; private set; }
        public virtual FacilityType FacilityType { get; private set; }

        public string TelNumber { get; private set; }
        public string MobileNumber { get; private set; }
        public string FaxNumber { get; private set; }
        
        public int? OrgUnitId { get; private set; }
        public virtual OrgUnit OrgUnit { get; set; }

		public virtual ICollection<PatientFacility> PatientFacilities { get; set; }
        public virtual ICollection<UserFacility> UserFacilities { get; set; }

        public string DisplayName
        {
            get
            {
                return $"{FacilityName} ({FacilityCode})";
            }
        }

        public void ChangeDetails(string facilityName, string facilityCode, FacilityType facilityType, string telNumber, string mobileNumber, string faxNumber, OrgUnit orgUnit)
        {
            FacilityName = facilityName;
            FacilityCode = facilityCode;

            FacilityTypeId = facilityType.Id;
            FacilityType = facilityType;

            TelNumber = telNumber;
            MobileNumber = mobileNumber;
            FaxNumber = faxNumber;

            OrgUnitId = orgUnit?.Id;
            OrgUnit = orgUnit;
        }
    }
}