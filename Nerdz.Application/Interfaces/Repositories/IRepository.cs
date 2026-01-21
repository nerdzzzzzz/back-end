using Nerdz.Domain.Entities;
using System.Linq.Expressions;

namespace Nerdz.Application.Interfaces.Repositories
{
    public interface IRepository<T> where T : BaseDomain
    {
        Task<T> GetAsync(long id, CancellationToken cancellation);
        Task<T> GetFirstAsync(Expression<Func<T, bool>> filter, CancellationToken cancellation);
        Task<IEnumerable<T>> ListAsync(CancellationToken cancellation);
        Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> filter, CancellationToken cancellation);
        Task<int> CountAsync(Expression<Func<T, bool>> filter, CancellationToken cancellation);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> filter, CancellationToken cancellation);
        Task SaveOrUpdateAsync(T item, CancellationToken cancellation);
        Task SaveOrUpdateAsync(IEnumerable<T> itens, CancellationToken cancellation);
        Task DeleteAsync(T item, CancellationToken cancellation);
        Task DeleteAsync(Expression<Func<T, bool>> filter, CancellationToken cancellation);
    }
}
