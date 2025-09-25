using Finances.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finances.Interfaces
{
    public interface ISummaryService
    {
        Task<SummaryResponseDto> GetAsync(
            SummaryPeriod period = SummaryPeriod.monthly,
            DateOnly? refDate = null,
            CancellationToken ct = default);
    }
}
