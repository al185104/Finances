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
    public sealed class SummaryService : BaseApiService, ISummaryService
    {
        public SummaryService(HttpClient http) : base(http) { }

        public async Task<SummaryResponseDto> GetAsync(
            SummaryPeriod period = SummaryPeriod.monthly, DateOnly? refDate = null, CancellationToken ct = default)
        {
            var qb = HttpUtility.ParseQueryString(string.Empty);
            qb["period"] = period.ToString(); // daily/weekly/monthly/yearly
            if (refDate is not null) qb["ref_date"] = refDate.Value.ToString("yyyy-MM-dd");

            var path = $"summary?{qb}";
            var resp = await Http.GetAsync(path, ct).ConfigureAwait(false);
            return await ReadAsAsync<SummaryResponseDto>(resp, Json).ConfigureAwait(false);
        }
    }
}
