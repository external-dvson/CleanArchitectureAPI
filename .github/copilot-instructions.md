# Copilot Instructions for Clean Architecture API

## Architecture Overview
This is a .NET 9 ASP.NET Core Web API implementing **Clean Architecture** with **Domain-Driven Design (DDD)** and **CQRS** using MediatR. The project strictly follows dependency inversion - outer layers depend on inner layers, never the reverse.

### Layer Dependencies (Critical)
```
Domain (no dependencies) ← Application ← Infrastructure ← Web
```
- **Domain**: Entities, repository interfaces, domain logic (`CleanArchitectureApi.Domain`)
- **Application**: CQRS handlers, DTOs, business logic (`CleanArchitectureApi.Application`)  
- **Infrastructure**: EF Core, repository implementations (`CleanArchitectureApi.Infrastructure`)
- **Web**: Controllers, DI configuration (`CleanArchitectureApi.Web`)

## Key Patterns & Conventions

### 1. CQRS with MediatR
All business operations go through MediatR handlers. Controllers only mediate:
```csharp
// Controller pattern - inject IMediator, send commands/queries
public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
{
    var result = await _mediator.Send(new CreateUserCommand(request.Username));
    return result.IsSuccess ? CreatedAtAction(...) : BadRequest(result.Error);
}
```

### 2. Feature Organization
Commands and queries are organized by feature in `Application/Features/{Entity}/{Commands|Queries}/{Operation}/`:
```
Features/Users/Commands/CreateUser/CreateUserCommand.cs
Features/Users/Queries/GetUserById/GetUserByIdQuery.cs
Features/Posts/Commands/CreatePost/CreatePostCommand.cs
```

### 3. Result Pattern
All application operations return `Result<T>` or `Result` for error handling:
```csharp
public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
{
    if (await _unitOfWork.Users.UsernameExistsAsync(request.Username))
        return Result<UserDto>.Failure("Username already exists");
    
    // ... business logic
    return Result<UserDto>.Success(userDto);
}
```

### 4. Repository + Unit of Work Pattern
- Use `IUnitOfWork` for coordinating repositories and transactions
- All read operations use `AsNoTracking()` and `AsSplitQuery()` for performance
- Repository methods follow naming: `GetByIdAsync`, `GetWithProfileAsync`, `UsernameExistsAsync`

### 5. EF Core Performance Patterns
**Critical**: Always use these optimizations for read operations:
```csharp
return await _dbSet
    .AsNoTracking()           // First - disable change tracking
    .AsSplitQuery()           // Prevent Cartesian explosion  
    .Where(...)               // Apply filters early
    .Include(...)             // Load related data last
    .FirstOrDefaultAsync(...);
```

## Development Workflows

### Adding New Features
1. **Domain**: Create entity in `Domain/Entities/`, add repository interface in `Domain/Repositories/`
2. **Application**: Create command/query in `Features/{Entity}/{Commands|Queries}/{Operation}/`
3. **Infrastructure**: Implement repository in `Infrastructure/Repositories/`, add EF configuration in `Data/Configurations/`
4. **Web**: Add controller endpoint calling MediatR

### Database Operations
```bash
# Add migration (run from Infrastructure project directory)
dotnet ef migrations add MigrationName --startup-project ../CleanArchitectureApi.Web

# Update database
dotnet ef database update --startup-project ../CleanArchitectureApi.Web
```

### Build & Run
```bash
# Build solution
dotnet build

# Run API (from Web project)
dotnet run --project src/CleanArchitectureApi.Web

# Test API endpoints
powershell -ExecutionPolicy Bypass -File test-api.ps1
```

## Dependency Injection Configuration

### Layer Registration Pattern
Each layer has a `DependencyInjection.cs` with extension methods:
- `services.AddApplication()` - registers MediatR
- `services.AddInfrastructure(configuration)` - registers DbContext, repositories, UnitOfWork

### Repository Registration
Use generic + specific pattern:
```csharp
services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
services.AddScoped<IUserRepository, UserRepository>();
```

## Entity Relationships & Domain Rules
- **User ↔ UserProfile**: One-to-one (each user has optional profile)
- **User ↔ Posts**: One-to-many (user can have multiple posts)  
- **Post ↔ Tags**: Many-to-many via `PostTag` junction table
- **Aggregate Roots**: User, Post, Tag (implement `IAggregateRoot`)

## Performance Guidelines
- **Read queries**: Always use `AsNoTracking()` + `AsSplitQuery()` for complex includes
- **Multiple includes**: Required to prevent Cartesian explosion (500 rows → 61 rows)
- **Query method order**: `AsNoTracking() → AsSplitQuery() → Where() → Include()`
- **Indexes**: Username, UserId, TagName are indexed for performance

## Testing & Validation
- API testing script: `test-api.ps1` demonstrates all endpoints
- FluentValidation used for input validation (see `CreateUserCommandValidator`)
- Error responses follow consistent `Result<T>` pattern
- Swagger UI available at `/swagger` for API exploration
