using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.Core.Aggregates.UserAggregate
{
    public enum UserType
    {
        [EnumMember(Value = "User")]
        User,
        [EnumMember(Value = "SystemAdmin")]
        SystemAdmin
    }
}
