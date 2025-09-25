using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Finances.Models
{
    public sealed class CategoryDto
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; } = "";
        [JsonPropertyName("amount_spent")] public decimal AmountSpent { get; set; } = 0m;
    }

    public sealed class CreateCategoryRequest
    {
        [JsonPropertyName("name")] public string Name { get; set; } = "";
    }
}
