using MediatR;
using PVIMS.Core.ValueTypes;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.ContactAggregate
{
    [DataContract]
    public class ChangeContactDetailsCommand
        : IRequest<bool>
    {
        [DataMember]
        public int Id { get; private set; }

        [DataMember]
        public OrganisationType OrganisationType { get; private set; }

        [DataMember]
        public string OrganisationName { get; private set; }

        [DataMember]
        public string DepartmentName { get; private set; }

        [DataMember]
        public string ContactFirstName { get; private set; }

        [DataMember]
        public string ContactSurname { get; private set; }

        [DataMember]
        public string StreetAddress { get; private set; }

        [DataMember]
        public string City { get; private set; }

        [DataMember]
        public string State { get; private set; }

        [DataMember]
        public string PostCode { get; private set; }

        [DataMember]
        public string CountryCode { get; private set; }

        [DataMember]
        public string ContactNumber { get; private set; }

        [DataMember]
        public string ContactEmail { get; private set; }

        public ChangeContactDetailsCommand()
        {
        }

        public ChangeContactDetailsCommand(int id, OrganisationType organisationType, string organisationName, string departmentName, string contactFirstName, string contactSurname, string streetAddress, string city, string state, string postCode, string countryCode, string contactNumber, string contactEmail): this()
        {
            Id = id;
            OrganisationType = organisationType;
            OrganisationName = organisationName;
            DepartmentName = departmentName;
            ContactFirstName = contactFirstName;
            ContactSurname = contactSurname;
            StreetAddress = streetAddress;
            City = city;
            State = state;
            PostCode = postCode;
            CountryCode = countryCode;
            ContactNumber = contactNumber;
            ContactEmail = contactEmail;
        }
    }
}
