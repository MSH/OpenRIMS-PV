using OpenRIMS.PV.Main.API.Infrastructure.Attributes;
using System;

namespace OpenRIMS.PV.Main.API.Models.Parameters
{
    public class AuditLogResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Specify the order you would like audit log transactions returned in  
        /// Default order attribute is ActionDate  
        /// Attribute must appear in payload to be sortable
        /// </summary>
        public string OrderBy { get; set; } = "ActionDate";

        /// <summary>
        /// The type of audit transaction to query
        /// </summary>
        [ValidEnumValue]
        public AuditTypeFilter AuditType { get; set; } = AuditTypeFilter.UserLogin;

        /// <summary>
        /// Filter audit logs by range
        /// </summary>
        public DateTime SearchFrom { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Filter audit logs by range
        /// </summary>
        public DateTime SearchTo { get; set; } = DateTime.MaxValue;

        /// <summary>
        /// Filter audit logs by facility
        /// </summary>
        public int FacilityId { get; set; } = 0;
    }

    public enum AuditTypeFilter
    {
        SubscriberAccess = 1,
        SubscriberPost = 2,
        MeddraImport = 3,
        UserLogin = 4,
        SynchronisationSuccessful = 5,
        SynchronisationError = 6,
        DataValidation = 7
    }
}
