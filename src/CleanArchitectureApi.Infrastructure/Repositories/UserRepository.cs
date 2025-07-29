using CleanArchitectureApi.Domain.Entities;
using CleanArchitectureApi.Domain.Repositories;
using CleanArchitectureApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitectureApi.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<User?> GetWithProfileAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetWithPostsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AsSplitQuery() // Use split query for multiple includes to avoid Cartesian explosion
            .Include(u => u.Posts)
                .ThenInclude(p => p.PostTags)
                    .ThenInclude(pt => pt.Tag)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(u => u.Username == username, cancellationToken);
    }

    /// <summary>
    /// High-performance method to get user with posts using projection to avoid loading unnecessary data
    /// </summary>
    public async Task<User?> GetWithPostsOptimizedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AsSplitQuery()
            .Where(u => u.Id == id)
            .Select(u => new User
            {
                Id = u.Id,
                Username = u.Username,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                Posts = u.Posts.Select(p => new Post
                {
                    Id = p.Id,
                    Title = p.Title,
                    UserId = p.UserId,
                    CreatedAt = p.CreatedAt,
                    PostTags = p.PostTags.Select(pt => new PostTag
                    {
                        PostId = pt.PostId,
                        TagId = pt.TagId,
                        Tag = new Tag
                        {
                            Id = pt.Tag.Id,
                            Name = pt.Tag.Name
                        }
                    }).ToList()
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
