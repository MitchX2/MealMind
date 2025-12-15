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
    // The API Lookup can reture 1 or many meals in a response
    // this wrapper wil allow us to take in the response and handle both cases
    public class MealLookupResponse
    {
        [JsonPropertyName("meals")]
        public List<MealDTO>? Meals { get; set; }
    }
}
