using CleanArchitectureApi.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitectureApi.Domain.Repositories;

public interface ITagRepository : IRepository<Tag>
{
    Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);
}
