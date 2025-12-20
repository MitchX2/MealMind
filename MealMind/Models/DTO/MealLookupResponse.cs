using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MealMind.Models;

namespace MealMind.Models.DTO
{
    // Wrapper for TheMealDB lookup/random API response.
    // The API always returns a "meals" array, even when only one meal is requested.
    public class MealLookupResponse
    {
        [JsonPropertyName("meals")]
        public List<MealLookupDto>? Meals { get; set; }
    }
}
