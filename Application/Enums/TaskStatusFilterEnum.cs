using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Application.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TaskStatusFilterEnum
    {
        [EnumMember(Value = "ongoing")]
        Ongoing,

        [EnumMember(Value = "completed")]
        Completed
    }
}