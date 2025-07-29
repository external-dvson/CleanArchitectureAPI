# Clean Architecture API (.NET 9)

A comprehensive ASP.NET Core Web API project implementing Clean Architecture principles with Domain-Driven Design (DDD) and Command Query Responsibility Segregation (CQRS) using MediatR.

## Architecture Overview

This project follows Clean Architecture principles with the following layers:

### 1. **Domain Layer** (`CleanArchitectureApi.Domain`)
- Contains business entities, domain logic, and repository interfaces
- No dependencies on external frameworks
- Implements DDD concepts with aggregate roots and domain entities

### 2. **Application Layer** (`CleanArchitectureApi.Application`)
- Contains application business logic using CQRS pattern
- Implements MediatR for handling Commands and Queries
- Contains DTOs and application services
- Depends only on Domain layer

### 3. **Infrastructure Layer** (`CleanArchitectureApi.Infrastructure`)
- Contains data access implementation using Entity Framework Core
- Implements repository pattern and Unit of Work
- Contains database configurations and migrations
- Depends on Domain and Application layers

### 4. **Presentation Layer** (`CleanArchitectureApi.Web`)
- ASP.NET Core Web API controllers
- Swagger/OpenAPI documentation
- Dependency injection container setup
- Depends on Application and Infrastructure layers

## Key Features

✅ **Clean Architecture** - Proper separation of concerns and dependency inversion  
✅ **Domain-Driven Design** - Rich domain models with business logic  
✅ **CQRS Pattern** - Separate read and write operations using MediatR  
✅ **Repository Pattern** - Abstracted data access layer  
✅ **Unit of Work** - Transaction management and data consistency  
✅ **Entity Framework Core** - Code-first approach with migrations  
✅ **Auto-Registration DI** - Automatic dependency injection setup  
✅ **SOLID Principles** - Maintainable and extensible code structure  
✅ **Performance Optimized** - Async/await, tracking optimization, proper indexing  

## Domain Entities

### User
- `Id` (Guid) - Primary key
- `Username` (string) - Unique username
- `Profile` (UserProfile) - One-to-one relationship
- `Posts` (ICollection<Post>) - One-to-many relationship

### UserProfile
- `Id` (Guid) - Primary key
- `Bio` (string) - User biography
- `UserId` (Guid) - Foreign key to User
- `User` (User) - Navigation property

### Post
- `Id` (Guid) - Primary key
- `Title` (string) - Post title
- `UserId` (Guid) - Foreign key to User
- `User` (User) - Navigation property
- `PostTags` (ICollection<PostTag>) - Many-to-many with Tags

### Tag
- `Id` (Guid) - Primary key
- `Name` (string) - Unique tag name
- `PostTags` (ICollection<PostTag>) - Many-to-many with Posts

### PostTag (Junction Table)
- `PostId` (Guid) - Foreign key to Post
- `TagId` (Guid) - Foreign key to Tag

## API Endpoints

### Users
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create new user

### Posts
- `GET /api/posts` - Get all posts with tags and user info
- `POST /api/posts` - Create new post with tags

## Technology Stack

- **.NET 9** - Latest .NET framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM for database operations
- **SQL Server** - Database (LocalDB for development)
- **MediatR** - CQRS implementation
- **Swagger/OpenAPI** - API documentation
- **AutoMapper** - Object mapping (recommended for complex scenarios)

## Getting Started

### Prerequisites
- .NET 9 SDK
- SQL Server LocalDB or SQL Server instance
- Visual Studio 2022 or VS Code

### Setup Instructions

1. **Clone the repository**
   ```bash
   git clone <your-repo-url>
   cd CleanArchitectureApi
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Update database connection string** (if needed)
   Edit `appsettings.json` in the Web project:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Data Source=HC-C-005ER\\LOCALHOST;Initial Catalog=CleanArchitectureApiDb;User Id=sa;Password=01662063068a@A;TrustServerCertificate=True;"
     }
   }
   ```

4. **Run migrations to create database**
   ```bash
   dotnet ef database update --startup-project src/CleanArchitectureApi.Web --project src/CleanArchitectureApi.Infrastructure
   ```

5. **Run the application**
   ```bash
   cd src/CleanArchitectureApi.Web
   dotnet run
   ```

6. **Access Swagger UI**
   Open your browser and navigate to: `https://localhost:5001/swagger` or `http://localhost:5000/swagger`

## Development Guidelines

### Adding New Features

1. **Create Domain Entity** (if needed)
   - Add to `Domain/Entities`
   - Implement aggregate root if applicable
   - Add repository interface to `Domain/Repositories`

2. **Create Application Features**
   - Add Commands in `Application/Features/{Entity}/Commands`
   - Add Queries in `Application/Features/{Entity}/Queries`
   - Create DTOs in `Application/DTOs`

3. **Implement Infrastructure**
   - Add repository implementation in `Infrastructure/Repositories`
   - Add entity configuration in `Infrastructure/Data/Configurations`

4. **Create API Controllers**
   - Add controller in `Web/Controllers`
   - Inject MediatR and call appropriate handlers

### Database Migrations

To add a new migration:
```bash
dotnet ef migrations add {MigrationName} --startup-project src/CleanArchitectureApi.Web --project src/CleanArchitectureApi.Infrastructure
```

To update database:
```bash
dotnet ef database update --startup-project src/CleanArchitectureApi.Web --project src/CleanArchitectureApi.Infrastructure
```

## Performance Considerations

- **AsNoTracking()** used for all read-only queries (20-40% performance improvement)
- **AsSplitQuery()** used for complex includes to prevent Cartesian explosion (40-70% improvement for complex queries)
- **Proper indexing** on frequently queried columns (Username, UserId, etc.)
- **Async/await** throughout the application
- **Strategic eager loading** with Include() statements when needed
- **Query optimization** with proper method ordering (AsNoTracking → AsSplitQuery → Where → Include)
- **Projection** available for heavy data transfer scenarios

### Query Performance Optimizations:
```csharp
// Optimized repository queries
return await _dbSet
    .AsNoTracking()           // Disable change tracking for reads
    .AsSplitQuery()           // Prevent Cartesian explosion  
    .Include(u => u.Posts)    // Eager load related data
        .ThenInclude(p => p.PostTags)
            .ThenInclude(pt => pt.Tag)
    .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
```

For detailed performance optimizations, see [PERFORMANCE_OPTIMIZATIONS.md](PERFORMANCE_OPTIMIZATIONS.md).

## Testing

The solution is designed to be easily testable:
- **Unit Tests** - Test domain logic and application handlers
- **Integration Tests** - Test API endpoints and database interactions
- **Repository Tests** - Test data access layer

## Contributing

1. Follow SOLID principles
2. Maintain Clean Architecture boundaries
3. Write unit tests for new features
4. Update documentation as needed
5. Use meaningful commit messages

## License

This project is licensed under the MIT License.
