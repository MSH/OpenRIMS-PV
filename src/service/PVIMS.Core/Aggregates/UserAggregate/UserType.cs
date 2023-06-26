using System.Runtime.Serialization;

namespace PVIMS.Core.Aggregates.UserAggregate
{
    public enum UserType
    {
        [EnumMember(Value = "User")]
        User,
        [EnumMember(Value = "SystemAdmin")]
        SystemAdmin
    }
}
