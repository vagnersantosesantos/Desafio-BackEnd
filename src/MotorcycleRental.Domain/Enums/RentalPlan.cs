using System.Text.Json.Serialization;

namespace Domain.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RentalPlan
    {
        SevenDays = 7,
        FifteenDays = 15,
        ThirtyDays = 30,
        FortyFiveDays = 45,
        FiftyDays = 50
    }
}
