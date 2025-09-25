using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Finances.Models
{
    public sealed class SummaryResponseDto
    {
        [JsonPropertyName("period")] public string Period { get; set; } = "";
        [JsonPropertyName("start_date")] public DateOnly StartDate { get; set; }
        [JsonPropertyName("end_date")] public DateOnly EndDate { get; set; }
        [JsonPropertyName("total_income")] public double TotalIncome { get; set; }
        [JsonPropertyName("total_spent")] public double TotalSpent { get; set; }
        [JsonPropertyName("net")] public double Net { get; set; }
    }
}
