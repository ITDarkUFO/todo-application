using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Application.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TasksFilterEnum
    {
        [Display(Name = "По умолчанию")]
        [EnumMember(Value = "default")]
        Default,

        [Display(Name = "По пользователям")]
        [EnumMember(Value = "user")]
        User,

        [Display(Name = "По выполнению")]
        [EnumMember(Value = "status")]
        Status,

        [Display(Name = "По приоритету")]
        [EnumMember(Value = "priority")]
        Priority
    }
}
