using CleanArchitectureApi.Application.Common.Interfaces;
using CleanArchitectureApi.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitectureApi.Application.Common.Behaviors;

/// <summary>
/// MediatR Pipeline Behavior that automatically wraps all commands in a database transaction.
/// This ensures all operations within a command are atomic and can be rolled back if any part fails.
/// </summary>
/// <typeparam name="TRequest">The request type (must be a command)</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(IUnitOfWork unitOfWork, ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Only apply transaction for Commands (not Queries)
        // Check if request implements ITransactionalCommand or follows naming convention
        var requestName = typeof(TRequest).Name;
        
        if (!IsTransactionalCommand(request, requestName))
        {
            // For queries and non-transactional commands, just pass through without transaction
            return await next();
        }

        _logger.LogInformation("Starting transaction for command: {CommandName}", requestName);

        try
        {
            // Begin transaction
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            
            _logger.LogDebug("Transaction started for command: {CommandName}", requestName);

            // Execute the command handler
            var response = await next();

            // Commit transaction if everything succeeded
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            
            _logger.LogInformation("Transaction committed successfully for command: {CommandName}", requestName);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in command: {CommandName}. Rolling back transaction.", requestName);

            try
            {
                // Rollback transaction on any error
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogInformation("Transaction rolled back successfully for command: {CommandName}", requestName);
            }
            catch (Exception rollbackEx)
            {
                _logger.LogError(rollbackEx, "Error occurred while rolling back transaction for command: {CommandName}", requestName);
            }

            // Re-throw the original exception
            throw;
        }
    }

    /// <summary>
    /// Determines if the request should be wrapped in a transaction.
    /// Checks both interface implementation and naming convention.
    /// </summary>
    /// <param name="request">The request instance</param>
    /// <param name="requestName">The name of the request type</param>
    /// <returns>True if this should be wrapped in a transaction</returns>
    private static bool IsTransactionalCommand(TRequest request, string requestName)
    {
        // First check: Does it implement ITransactionalCommand interface?
        if (request is ITransactionalCommand<TResponse> || request is ITransactionalCommand)
        {
            return true;
        }

        // Fallback: Convention-based check - Commands end with "Command"
        return requestName.EndsWith("Command", StringComparison.OrdinalIgnoreCase);
    }
}