using System;

namespace CleanArchitectureApi.Domain.Common;

public interface IAggregateRoot
{
    Guid Id { get; }
}
