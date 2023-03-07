namespace PoliticiansAndParties.Api.Logging;

public class LoggerAdapter<TType> : ILoggerAdapter<TType>
{
    private readonly ILogger<TType> _logger;

    public LoggerAdapter(ILogger<TType> logger)
    {
        _logger = logger;
    }

    public void LogDebug(string? message, params object?[] args)
    {
        _logger.LogDebug(message, args);
    }

    public void LogError(Exception? exception, string? message, params object?[] args)
    {
        _logger.LogError(exception, message, args);
    }

    public void LogInfo(string? message, params object?[] args)
    {
        _logger.LogInformation(message, args);
    }

    public void LogWarn(string? message, params object?[] args)
    {
        _logger.LogWarning(message, args);
    }
}
