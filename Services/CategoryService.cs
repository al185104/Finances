using Finances.Interfaces;
using Finances.Models;
using Finances.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Finances.Services
{
    public sealed class CategoryService : BaseApiService, ICategoryService
    {
        public CategoryService(HttpClient http) : base(http) { }

        public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken ct = default)
        {
            var resp = await Http.GetAsync("categories", ct).ConfigureAwait(false);
            return await ReadAsAsync<List<CategoryDto>>(resp, Json).ConfigureAwait(false);
        }

        public async Task<CategoryDto> CreateAsync(string name, CancellationToken ct = default)
        {
            var payload = new CreateCategoryRequest { Name = name };
            var resp = await Http.PostAsync("categories", ToJsonContent(payload, Json), ct).ConfigureAwait(false);
            return await ReadAsAsync<CategoryDto>(resp, Json).ConfigureAwait(false);
        }
    }
}
