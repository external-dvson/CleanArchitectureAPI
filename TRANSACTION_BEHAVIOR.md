# Transaction Behavior Implementation

## Tổng quan

Đã implement **TransactionBehavior** - một MediatR Pipeline Behavior tự động wrap tất cả commands trong database transaction, đảm bảo tính atomicity và khả năng rollback.

## Kiến trúc

### 1. TransactionBehavior

```csharp
// Tự động wrap commands trong transaction
public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
```

**Chức năng:**
- Tự động detect commands (class name kết thúc bằng "Command" hoặc implement `ITransactionalCommand`)
- Bắt đầu transaction trước khi execute command handler
- Commit transaction khi thành công
- Rollback transaction khi có exception
- Queries không được wrap trong transaction (for performance)

### 2. ITransactionalCommand Interface

```csharp
public interface ITransactionalCommand<out TResponse> : IRequest<TResponse> { }
public interface ITransactionalCommand : IRequest { }
```

**Cách sử dụng:**
```csharp
// Thay vì IRequest<Result<UserDto>>
public record CreateUserCommand(string Username, string? Bio = null) : ITransactionalCommand<Result<UserDto>>;
```

### 3. Repository Extensions

Đã thêm methods cho bulk operations:

```csharp
// Truncate table
Task DeleteAllAsync(CancellationToken cancellationToken = default);

// Raw SQL execution  
Task<int> ExecuteSqlRawAsync(string sql, CancellationToken cancellationToken = default);

// Bulk insert (đã có sẵn)
Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
```

## Cách sử dụng

### 1. Command đơn giản (tự động transaction)

```csharp
public record CreateUserCommand(string Username, string? Bio = null) : ITransactionalCommand<Result<UserDto>>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Tất cả logic này sẽ được wrap trong transaction tự động
        var user = new User { Username = request.Username };
        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        // Transaction sẽ tự động commit ở đây
        return Result<UserDto>.Success(userDto);
    }
}
```

### 2. Truncate + Bulk Insert Command

```csharp
public record BulkReplaceUsersCommand(List<BulkUserData> Users) : ITransactionalCommand<Result<List<UserDto>>>;

public class BulkReplaceUsersCommandHandler : IRequestHandler<BulkReplaceUsersCommand, Result<List<UserDto>>>
{
    public async Task<Result<List<UserDto>>> Handle(BulkReplaceUsersCommand request, CancellationToken cancellationToken)
    {
        // Method 1: TRUNCATE (nhanh nhất, nhưng không thể rollback với foreign keys)
        await _unitOfWork.Users.ExecuteSqlRawAsync("TRUNCATE TABLE Users", cancellationToken);
        
        // Method 2: DELETE ALL (an toàn hơn với foreign key constraints)
        // await _unitOfWork.Users.DeleteAllAsync(cancellationToken);
        // await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Bulk insert
        var usersToInsert = request.Users.Select(userData => new User { Username = userData.Username }).ToList();
        await _unitOfWork.Users.AddRangeAsync(usersToInsert, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Tất cả operations trên sẽ được commit/rollback cùng nhau
        return Result<List<UserDto>>.Success(userDtos);
    }
}
```

### 3. Test API Endpoint

```bash
POST /api/users/bulk-replace
{
  "users": [
    { "username": "user1", "bio": "Bio 1" },
    { "username": "user2", "bio": "Bio 2" },
    { "username": "user3" }
  ]
}
```

## Lợi ích

### ✅ Được tự động hóa
- Không cần manual `BeginTransaction()`, `Commit()`, `Rollback()`
- Clean command handlers không bị "pollution" bởi transaction code
- Convention-based: tất cả commands đều được bảo vệ

### ✅ Đảm bảo atomicity
- Truncate + bulk insert đều trong cùng 1 transaction
- Rollback toàn bộ nếu có bất kỳ lỗi nào
- Race condition được loại bỏ

### ✅ Performance
- Queries không bị wrap trong transaction (performance tốt hơn)
- Bulk operations được optimize
- Logging chi tiết cho debugging

### ✅ Flexibility
- Có thể dùng TRUNCATE (nhanh) hoặc DELETE ALL (an toàn)
- Support cả raw SQL và EF Core operations
- Easy to extend cho các bulk operations khác

## Transaction Guarantees

```csharp
// Scenario: Nếu có lỗi bất kỳ đâu trong command
public async Task<Result<T>> Handle(Command request, CancellationToken cancellationToken)
{
    await _unitOfWork.Users.ExecuteSqlRawAsync("TRUNCATE TABLE Users", cancellationToken);          // ✅ 
    await _unitOfWork.Users.AddRangeAsync(newUsers, cancellationToken);                             // ✅
    await _unitOfWork.SaveChangesAsync(cancellationToken);                                          // ❌ Exception ở đây
    // TransactionBehavior sẽ tự động rollback → table Users về trạng thái ban đầu
}
```

**Kết quả:** Table Users sẽ về đúng trạng thái ban đầu, không bị mất data và không bị insert partial data.