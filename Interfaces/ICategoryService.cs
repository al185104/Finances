using Finances.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finances.Interfaces
{
    public interface ICategoryService
    {
        Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken ct = default);
        Task<CategoryDto> CreateAsync(string name, CancellationToken ct = default);
    }
}
