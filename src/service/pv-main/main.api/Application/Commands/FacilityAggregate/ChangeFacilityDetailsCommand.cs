using MediatR;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.FacilityAggregate
{
    [DataContract]
    public class ChangeFacilityDetailsCommand
        : IRequest<bool>
    {
        [DataMember]
        public int Id { get; private set; }

        [DataMember]
        public string FacilityName { get; private set; }

        [DataMember]
        public string FacilityCode { get; private set; }

        [DataMember]
        public string FacilityType { get; private set; }

        [DataMember]
        public string TelNumber { get; private set; }

        [DataMember]
        public string MobileNumber { get; private set; }

        [DataMember]
        public string FaxNumber { get; private set; }

        [DataMember]
        public int? OrgUnitId { get; private set; }

        public ChangeFacilityDetailsCommand()
        {
        }

        public ChangeFacilityDetailsCommand(int id, string facilityName, string facilityCode, string facilityType, string telNumber, string mobileNumber, string faxNumber, int? orgUnitId) : this()
        {
            Id = id;
            FacilityName = facilityName;
            FacilityCode = facilityCode;
            FacilityType = facilityType;
            TelNumber = telNumber;
            MobileNumber = mobileNumber;
            FaxNumber = faxNumber;
            OrgUnitId = orgUnitId;
        }
    }
}
