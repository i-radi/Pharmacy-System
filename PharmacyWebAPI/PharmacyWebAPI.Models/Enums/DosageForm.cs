using System.Text.Json.Serialization;

namespace PharmacyWebAPI.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DosageForm
    {
        Tablet = 1,
        Inhaler = 2,
        Capsule = 3,
        Patch = 4,
        Injection = 5,
        Syrup = 6,
        Ointment = 7
    }
}