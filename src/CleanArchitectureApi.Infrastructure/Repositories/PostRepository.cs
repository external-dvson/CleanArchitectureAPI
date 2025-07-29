using CleanArchitectureApi.Domain.Entities;
using CleanArchitectureApi.Domain.Repositories;
using CleanArchitectureApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitectureApi.Infrastructure.Repositories;

public class PostRepository : Repository<Post>, IPostRepository
{
    public PostRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Post>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AsSplitQuery() // Split query for better performance with multiple includes
            .Where(p => p.UserId == userId)
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .ToListAsync(cancellationToken);
    }

    public async Task<Post?> GetWithTagsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AsSplitQuery() // Split query to avoid Cartesian explosion
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Post>> GetWithUserAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AsSplitQuery() // Split query for better performance
            .Include(p => p.User)
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Post>> GetByTagAsync(string tagName, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AsSplitQuery() // Split query for complex includes
            .Where(p => p.PostTags.Any(pt => pt.Tag.Name == tagName))
            .Include(p => p.User)
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .ToListAsync(cancellationToken);
    }
}
