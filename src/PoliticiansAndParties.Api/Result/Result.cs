using PoliticiansAndParties.Api.Models;

namespace PoliticiansAndParties.Api.Result;

/// <summary>
///     Result representation
/// </summary>
/// <typeparam name="T">Result's data if result is without error</typeparam>
public class Result<T>
{
    /// <summary>
    ///     Empty error result only with error type
    /// </summary>
    /// <param name="errorType"></param>
    public Result(ErrorType errorType)
    {
        IsError = true;
        Error = null;
        ErrorType = errorType;
    }

    /// <summary>
    ///     Initializes an error result with an error detail
    /// </summary>
    /// <param name="error">List of error descriptions</param>
    /// <param name="errorType">Type of error</param>
    public Result(ErrorDetail error, ErrorType errorType = ErrorType.Invalid)
    {
        IsError = true;
        Error = error;
        ErrorType = errorType;
    }

    /// <summary>
    ///     Initializes a successful result
    /// </summary>
    /// <param name="data">Data that are part of a result</param>
    public Result(T data)
    {
        IsError = false;
        Data = data;
    }

    public bool IsError { get; }

    public ErrorType ErrorType { get; }
    public T? Data { get; }

    public ErrorDetail? Error { get; }
}
