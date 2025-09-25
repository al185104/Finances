using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Finances.Models
{
    public sealed class TransactionDto
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("amount")] public double Amount { get; set; }
        [JsonPropertyName("txn_type")] public TxnType TxnType { get; set; }
        [JsonPropertyName("txn_date")] public DateOnly TxnDate { get; set; }
        [JsonPropertyName("description")] public string? Description { get; set; }
        [JsonPropertyName("category_id")] public int? CategoryId { get; set; }
        [JsonPropertyName("is_recurring")] public bool IsRecurring { get; set; }
        [JsonPropertyName("recurrence_period")] public RecurrencePeriod? RecurrencePeriod { get; set; }
        [JsonPropertyName("recurrence_interval")] public int? RecurrenceInterval { get; set; }
    }

    public sealed class CreateTransactionRequest
    {
        [JsonPropertyName("amount")] public double? Amount { get; set; }
        [JsonPropertyName("txn_type")] public TxnType TxnType { get; set; }
        [JsonPropertyName("txn_date")] public DateOnly TxnDate { get; set; }
        [JsonPropertyName("description")] public string? Description { get; set; }
        [JsonPropertyName("category_id")] public int? CategoryId { get; set; }
        [JsonPropertyName("is_recurring")] public bool IsRecurring { get; set; }
        [JsonPropertyName("recurrence_period")] public RecurrencePeriod? RecurrencePeriod { get; set; }
        [JsonPropertyName("recurrence_interval")] public int? RecurrenceInterval { get; set; } = 1;
    }

    public sealed class UpdateTransactionRequest
    {
        [JsonPropertyName("amount")] public double? Amount { get; set; }
        [JsonPropertyName("txn_type")] public TxnType? TxnType { get; set; }
        [JsonPropertyName("txn_date")] public DateOnly? TxnDate { get; set; }
        [JsonPropertyName("description")] public string? Description { get; set; }
        [JsonPropertyName("category_id")] public int? CategoryId { get; set; }
        [JsonPropertyName("is_recurring")] public bool? IsRecurring { get; set; }
        [JsonPropertyName("recurrence_period")] public RecurrencePeriod? RecurrencePeriod { get; set; }
        [JsonPropertyName("recurrence_interval")] public int? RecurrenceInterval { get; set; }
    }
}
