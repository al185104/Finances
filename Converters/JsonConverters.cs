using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Finances.Converters
{
    public sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private const string Format = "yyyy-MM-dd";

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => DateOnly.ParseExact(reader.GetString()!, Format);

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString(Format));
    }

    public sealed class NullableDateOnlyJsonConverter : JsonConverter<DateOnly?>
    {
        private const string Format = "yyyy-MM-dd";

        public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.TokenType == JsonTokenType.Null ? null : DateOnly.ParseExact(reader.GetString()!, Format);

        public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
        {
            if (value is null) { writer.WriteNullValue(); return; }
            writer.WriteStringValue(value.Value.ToString(Format));
        }
    }

    public sealed class FlexibleBoolConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.True: return true;
                case JsonTokenType.False: return false;
                case JsonTokenType.Number:
                    if (reader.TryGetInt32(out var n)) return n != 0;
                    throw new JsonException("Expected number 0/1 for boolean.");
                case JsonTokenType.String:
                    var s = reader.GetString()?.Trim().ToLowerInvariant();
                    if (s is "true" or "1") return true;
                    if (s is "false" or "0") return false;
                    throw new JsonException($"Cannot parse '{s}' as boolean.");
                default:
                    throw new JsonException($"Token {reader.TokenType} is not a boolean.");
            }
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
            => writer.WriteBooleanValue(value);
    }

    public sealed class FlexibleNullableBoolConverter : JsonConverter<bool?>
    {
        public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;

            // Delegate to non-nullable converter
            var conv = new FlexibleBoolConverter();
            return conv.Read(ref reader, typeof(bool), options);
        }

        public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
        {
            if (value is null) { writer.WriteNullValue(); return; }
            writer.WriteBooleanValue(value.Value);
        }
    }
}
