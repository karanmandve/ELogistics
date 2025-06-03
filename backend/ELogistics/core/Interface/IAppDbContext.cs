using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace core.Interface
{
    public interface IAppDbContext
    {
        DbSet<TEntity> Set<TEntity>()
           where TEntity : class;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
