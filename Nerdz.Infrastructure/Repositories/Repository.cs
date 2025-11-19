using Google.Api;
using Microsoft.EntityFrameworkCore;
using Nerdz.Application.Interfaces.Repositories;
using Nerdz.Domain.Entities;
using Nerdz.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace Nerdz.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseDomain
    {
        private readonly ApplicationDbContext session;
        private readonly DbSet<T> dbSet;

        public Repository(ApplicationDbContext session)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            this.dbSet = session.Set<T>();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> filter, CancellationToken cancellation)
        {
            if (filter is null)
                throw new ArgumentNullException(nameof(filter));

            return await dbSet.CountAsync(filter, cancellation);
        }

        public async Task DeleteAsync(T item, CancellationToken cancellation)
        {
            if (item == null) return;

            dbSet.Remove(item);
            await session.SaveChangesAsync(cancellation);
        }

        public async Task DeleteAsync(Expression<Func<T, bool>> filter, CancellationToken cancellation)
        {
            await dbSet.Where(filter).ExecuteDeleteAsync(cancellation);
        }

        public Task<bool> ExistsAsync(Expression<Func<T, bool>> filter, CancellationToken cancellation)
        {
            if (filter is null)
                throw new ArgumentNullException(nameof(filter));

            return dbSet.AnyAsync(filter, cancellation);
        }

        public async Task<T> GetAsync(long id, CancellationToken cancellation)
        {
            if (id <= 0)
                throw new ArgumentException("O id deve ser maior que zero.", nameof(id));

            return await dbSet.FindAsync(new object[] { id }, cancellation);
        }

        public Task<T> GetFirstAsync(Expression<Func<T, bool>> filter, CancellationToken cancellation)
        {
            if (filter is null)
                throw new ArgumentNullException(nameof(filter));

            return dbSet.FirstOrDefaultAsync(filter, cancellation);
        }

        public async Task<IEnumerable<T>> ListAsync(CancellationToken cancellation)
        {
            return await dbSet.ToListAsync(cancellation);
        }

        public async Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> filter, CancellationToken cancellation)
        {
            if (filter is null)
                throw new ArgumentNullException(nameof(filter));

            return await dbSet.Where(filter).ToListAsync(cancellation);
        }

        public async Task SaveOrUpdateAsync(T item, CancellationToken cancellationToken = default)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            if (item.Id == Guid.Empty)
            {
                await dbSet.AddAsync(item, cancellationToken);
            }
            else
            {
                dbSet.Update(item);
            }

            await session.SaveChangesAsync(cancellationToken);
        }

        public async Task SaveOrUpdateAsync(IEnumerable<T> items, CancellationToken cancellation)
        {
            if (items == null || !items.Any()) return;

            foreach (var item in items)
            {
                if (item.Id == Guid.Empty)
                {
                    await dbSet.AddAsync(item, cancellation);
                }
                else
                {
                    dbSet.Update(item);
                }
            }

            await session.SaveChangesAsync(cancellation);
        }
    }
}
