using CleanArchitectureApi.Domain.Entities;

namespace CleanArchitectureApi.Domain.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<IEnumerable<User>> GetUsersWithProfileAsync(CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetWithProfileAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetWithPostsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetWithProfileForUpdateAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateUserWithNewProfileAsync(User user, UserProfile profile, CancellationToken cancellationToken = default);
}
