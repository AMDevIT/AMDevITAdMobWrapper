using AMDevIT.Admob.Wrapper.iOSNative;

namespace AMDevIT.Admob.Wrapper.AppleTestApp.Diagnostics;

internal sealed class ConsoleAppleLogger : NSObject, IAppleLogger
{
    #region Methods

    public bool IsEnabled(AppleLogLevel level) => level != AppleLogLevel.None;

    public void LogTrace(string message, string? tag) =>
        Write(AppleLogLevel.Trace, message, tag);

    public void LogDebug(string message, string? tag) =>
        Write(AppleLogLevel.Debug, message, tag);

    public void LogInfo(string message, string? tag) =>
        Write(AppleLogLevel.Information, message, tag);

    public void LogWarning(string message, string? tag) =>
        Write(AppleLogLevel.Warning, message, tag);

    public void LogError(string message, string? tag) =>
        Write(AppleLogLevel.Error, message, tag);

    public void LogCritical(string message, string? tag) =>
        Write(AppleLogLevel.Critical, message, tag);

    private static void Write(AppleLogLevel level, string message, string? tag)
    {
        Console.WriteLine($"[{level}] {tag ?? "AdMobWrapper"}: {message}");
    }

    #endregion
}
