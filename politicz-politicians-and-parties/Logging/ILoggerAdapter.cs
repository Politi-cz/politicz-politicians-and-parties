namespace politicz_politicians_and_parties.Logging
{
    public interface ILoggerAdapter<TType>
    {
        void LogError(Exception? exception, string? message, params object?[] args);
        void LogWarn(string? message, params object?[] args);
        void LogInfo(string? message, params object?[] args);
        void LogDebug(string? message, params object?[] args);
    }
}
