using System;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Persistence.Services.Shared
{
    public class NewsCategoryService : INewsCategoryService
    {
        private readonly IApplicationDbContext _context;
        public NewsCategoryService(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CheckExistNameAsync(Guid? id, string name, CancellationToken cancellationToken)
        {
            return await _context.NewsCategories.AnyAsync(x => x.Id != id && x.Name == name, cancellationToken);
        }
    }
}
