using MediatR;

namespace CleanArchitectureApi.Application.Common.Interfaces;

/// <summary>
/// Marker interface for commands that should be executed within a database transaction.
/// All commands implementing this interface will automatically be wrapped in a transaction
/// by the TransactionBehavior pipeline.
/// </summary>
/// <typeparam name="TResponse">The response type returned by the command</typeparam>
public interface ITransactionalCommand<out TResponse> : IRequest<TResponse>
{
}

/// <summary>
/// Marker interface for commands that don't return a value but should be executed within a transaction.
/// </summary>
public interface ITransactionalCommand : IRequest
{
}