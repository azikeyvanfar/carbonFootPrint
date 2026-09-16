using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Ghg
{
    /// <summary>
    /// میان‌برهای async امن برای کوئری‌های EF Core در هندلرهای GHG
    /// </summary>
    public static class EfSafeExtensions
    {
        public static Task<List<T>> ToListAsyncSafe<T>(this IQueryable<T> query, CancellationToken cancellationToken = default)
            => query.ToListAsync(cancellationToken);

        public static Task<T?> FirstOrDefaultAsyncSafe<T>(this IQueryable<T> query, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) where T : class
            => query.FirstOrDefaultAsync(predicate, cancellationToken);

        public static Task<bool> AnyAsyncSafe<T>(this IQueryable<T> query, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => query.AnyAsync(predicate, cancellationToken);
    }
}
