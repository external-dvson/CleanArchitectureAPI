using CleanArchitectureApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitectureApi.Domain.Repositories;

public interface IPostRepository : IRepository<Post>
{
    Task<IEnumerable<Post>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Post?> GetWithTagsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetWithUserAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetByTagAsync(string tagName, CancellationToken cancellationToken = default);
}
