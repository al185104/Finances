using Finances.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Finances.Services.Base
{
    public abstract class BaseApiService
    {
        protected readonly HttpClient Http;
        protected readonly JsonSerializerOptions Json;

        protected BaseApiService(HttpClient http)
        {
            Http = http;

            Json = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            // this is just for formatting purposes, to clearly convert the API requests/responses
            Json.Converters.Add(new DateOnlyJsonConverter());
            Json.Converters.Add(new NullableDateOnlyJsonConverter());
            Json.Converters.Add(new FlexibleBoolConverter());
            Json.Converters.Add(new FlexibleNullableBoolConverter());
            Json.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        }

        protected static StringContent ToJsonContent<T>(T value, JsonSerializerOptions opts)
            => new(JsonSerializer.Serialize(value, opts), Encoding.UTF8, "application/json");

        protected static async Task<T> ReadAsAsync<T>(HttpResponseMessage resp, JsonSerializerOptions opts)
        {
            var content = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"{(int)resp.StatusCode} {resp.ReasonPhrase} | {content}");

            return JsonSerializer.Deserialize<T>(content, opts)!;
        }
    }
}
