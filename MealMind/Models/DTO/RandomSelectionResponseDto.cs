using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace MealMind.Models.DTO;

public class RandomSelectionResponseDto
{
    [JsonPropertyName("meals")]
    public List<MealLookupDto>? Meals { get; set; }
}
