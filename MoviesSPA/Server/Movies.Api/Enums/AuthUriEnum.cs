using System.Runtime.Serialization;

namespace Movies.Api.Enums;

public enum AuthUriEnum
{
    [EnumMember(Value = "register")]
    Register,
    [EnumMember(Value = "login")]
    Login,
}
