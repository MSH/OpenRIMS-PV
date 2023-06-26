using PVIMS.Core.Entities;
using PVIMS.Core.SeedWork;
using PVIMS.Core.ValueTypes;

namespace PVIMS.Core.Aggregates.ContactAggregate
{
    public class SiteContactDetail 
        : AuditedEntityBase, IAggregateRoot
    {
        protected SiteContactDetail()
        {
        }

        public SiteContactDetail(ContactType contactType, OrganisationType organisationType, string organisationName, string departmentName, string contactFirstName, string contactSurname, string streetAddress, string city, string state, string postCode, string countryCode, string contactNumber, string contactEmail)
        {
            ContactType = contactType;
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

        public ContactType ContactType { get; private set; }
        public OrganisationType OrganisationType { get; private set; }
        public string OrganisationName { get; private set; }
        public string DepartmentName { get; private set; }
        public string ContactFirstName { get; private set; }
        public string ContactSurname { get; private set; }
        public string StreetAddress { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string PostCode { get; private set; }
        public string CountryCode { get; private set; }
        public string ContactNumber { get; private set; }
        public string ContactEmail { get; private set; }

        public void ChangeDetails(OrganisationType organisationType, string organisationName, string departmentName, string contactFirstName, string contactSurname, string streetAddress, string city, string state, string postCode, string countryCode, string contactNumber, string contactEmail)
        {
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