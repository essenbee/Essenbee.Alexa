using System.Runtime.Serialization;

namespace Essenbee.Alexa.Lib.Request
{
    public enum Reason
    {
        [EnumMember(Value = "USER_INITIATED")]
        UserInitiated,
        [EnumMember(Value = "ERROR")]
        Error,
        [EnumMember(Value = "EXCEEDED_MAX_REPROMPTS")]
        ExceededMaxReprompts
    }
}
