using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PharmacyWebAPI.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PregnancyCategory
    {
        A = 1,
        B = 2,
        C = 3,
        D = 4,
        X = 5
    }
}