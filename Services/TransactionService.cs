using Finances.Interfaces;
using Finances.Models;
using Finances.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static System.Net.WebRequestMethods;

namespace Finances.Services
{
    public sealed class TransactionService : BaseApiService, ITransactionService
    {
        public TransactionService(HttpClient http) : base(http) { }

        public async Task<IReadOnlyList<TransactionDto>> ListAsync(
            SummaryPeriod period = SummaryPeriod.monthly, DateOnly? refDate = null,
            DateOnly? startDate = null, DateOnly? endDate = null, int? categoryId = null, CancellationToken ct = default)
        {
            var uri = BuildQuery("transactions", period, refDate, startDate, endDate, categoryId);
            var resp = await Http.GetAsync(uri, ct).ConfigureAwait(false);
            return await ReadAsAsync<List<TransactionDto>>(resp, Json).ConfigureAwait(false);
        }

        public async Task<TransactionDto> GetAsync(int id, CancellationToken ct = default)
        {
            var resp = await Http.GetAsync($"transactions/{id}", ct).ConfigureAwait(false);
            return await ReadAsAsync<TransactionDto>(resp, Json).ConfigureAwait(false);
        }

        public async Task<TransactionDto> CreateAsync(CreateTransactionRequest req, CancellationToken ct = default)
        {
            var resp = await Http.PostAsync("transactions", ToJsonContent(req, Json), ct).ConfigureAwait(false);
            return await ReadAsAsync<TransactionDto>(resp, Json).ConfigureAwait(false);
        }

        public async Task<TransactionDto> UpdateAsync(int id, UpdateTransactionRequest req, CancellationToken ct = default)
        {
            var resp = await Http.PutAsync($"transactions/{id}", ToJsonContent(req, Json), ct).ConfigureAwait(false);
            return await ReadAsAsync<TransactionDto>(resp, Json).ConfigureAwait(false);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var resp = await Http.DeleteAsync($"transactions/{id}", ct).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode)
            {
                var msg = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new HttpRequestException($"{(int)resp.StatusCode} {resp.ReasonPhrase} | {msg}");
            }
        }

        private static string BuildQuery(string basePath, SummaryPeriod period, DateOnly? refDate, DateOnly? start, DateOnly? end, int? catId)
        {
            var qb = HttpUtility.ParseQueryString(string.Empty);
            qb["period"] = period.ToString(); // daily/weekly/monthly/yearly
            if (refDate is not null) qb["ref_date"] = refDate.Value.ToString("yyyy-MM-dd");
            if (start is not null) qb["start_date"] = start.Value.ToString("yyyy-MM-dd");
            if (end is not null) qb["end_date"] = end.Value.ToString("yyyy-MM-dd");
            if (catId is not null) qb["category_id"] = catId.Value.ToString();
            var q = qb.ToString();
            return string.IsNullOrEmpty(q) ? basePath : $"{basePath}?{q}";
        }
    }
}
