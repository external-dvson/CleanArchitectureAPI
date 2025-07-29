using CleanArchitectureApi.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitectureApi.Domain.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetWithProfileAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetWithPostsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default);
}
