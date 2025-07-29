using System;

namespace CleanArchitectureApi.Application.Common.Models;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string? Error { get; private set; }
    public string[]? Errors { get; private set; }

    private Result(bool isSuccess, T? data, string? error, string[]? errors = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        Error = error;
        Errors = errors;
    }

    public static Result<T> Success(T data) => new(true, data, null);
    public static Result<T> Failure(string error) => new(false, default, error);
    public static Result<T> Failure(string[] errors) => new(false, default, null, errors);
}

public class Result
{
    public bool IsSuccess { get; private set; }
    public string? Error { get; private set; }
    public string[]? Errors { get; private set; }

    private Result(bool isSuccess, string? error, string[]? errors = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        Errors = errors;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);
    public static Result Failure(string[] errors) => new(false, null, errors);
}
