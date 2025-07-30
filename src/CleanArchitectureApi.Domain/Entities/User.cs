using CleanArchitectureApi.Domain.Common;

namespace CleanArchitectureApi.Domain.Entities;

public class User : BaseEntity, IAggregateRoot
{
    public string Username { get; set; } = string.Empty;

    // Navigation properties
    public UserProfile? Profile { get; set; }
    public ICollection<Post> Posts { get; set; } = new List<Post>();

    public void CreateProfile(string? bio = null)
    {
        if (Profile == null)
        {
            Profile = new UserProfile
            {
                UserId = Id,
                Bio = bio ?? string.Empty,
                User = this,
                CreatedAt = DateTime.UtcNow
            };
        }
    }

    public void UpdateProfile(string bio)
    {
        if (Profile == null)
        {
            CreateProfile(bio);
        }
        else
        {
            Profile.Bio = bio;
            Profile.UpdatedAt = DateTime.UtcNow;
        }
    }
}
