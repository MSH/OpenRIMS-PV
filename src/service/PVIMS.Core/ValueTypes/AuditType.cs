namespace PVIMS.Core.ValueTypes
{
    public enum AuditType
    {
        InvalidSubscriberAccess = 1,
        ValidSubscriberAccess = 2,
        InValidSubscriberPost = 3,
        ValidSubscriberPost = 4,
        UserLogin = 5,
        InValidMedDRAImport = 6,
        ValidMedDRAImport = 7,
        SynchronisationForm = 8,
        SynchronisationError = 9,
        DataValidation = 10
    }
}
