using Finances.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finances.Interfaces
{
    public interface ITransactionService
    {
        Task<IReadOnlyList<TransactionDto>> ListAsync(
            SummaryPeriod period = SummaryPeriod.monthly,
            DateOnly? refDate = null,
            DateOnly? startDate = null,
            DateOnly? endDate = null,
            int? categoryId = null,
            CancellationToken ct = default);

        Task<TransactionDto> GetAsync(int id, CancellationToken ct = default);
        Task<TransactionDto> CreateAsync(CreateTransactionRequest req, CancellationToken ct = default);
        Task<TransactionDto> UpdateAsync(int id, UpdateTransactionRequest req, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
