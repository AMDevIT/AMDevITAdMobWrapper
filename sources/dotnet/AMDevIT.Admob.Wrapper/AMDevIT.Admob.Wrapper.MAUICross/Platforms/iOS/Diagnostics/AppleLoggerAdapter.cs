#if IOS

using AMDevIT.Admob.Wrapper.iOSNative;
using Foundation;
using Microsoft.Extensions.Logging;

namespace AMDevIT.Admob.Wrapper.MAUICross.Platforms.iOS.Diagnostics;

internal sealed class AppleLoggerAdapter : NSObject, IAppleLogger
{
    #region Fields

    private readonly ILogger logger;

    #endregion

    #region .ctor

    public AppleLoggerAdapter(ILogger logger)
    {
        this.logger = logger;
    }

    #endregion

    #region Methods

    public bool IsEnabled(AppleLogLevel level) =>
        this.logger.IsEnabled(MapLogLevel(level));

    public void LogTrace(string message, string? tag) =>
        this.Log(Microsoft.Extensions.Logging.LogLevel.Trace, message, tag);

    public void LogDebug(string message, string? tag) =>
        this.Log(Microsoft.Extensions.Logging.LogLevel.Debug, message, tag);

    public void LogInfo(string message, string? tag) =>
        this.Log(Microsoft.Extensions.Logging.LogLevel.Information, message, tag);

    public void LogWarning(string message, string? tag) =>
        this.Log(Microsoft.Extensions.Logging.LogLevel.Warning, message, tag);

    public void LogError(string message, string? tag) =>
        this.Log(Microsoft.Extensions.Logging.LogLevel.Error, message, tag);

    public void LogCritical(string message, string? tag) =>
        this.Log(Microsoft.Extensions.Logging.LogLevel.Critical, message, tag);

    private static Microsoft.Extensions.Logging.LogLevel MapLogLevel(
        AppleLogLevel level) => level switch
    {
        AppleLogLevel.Trace => Microsoft.Extensions.Logging.LogLevel.Trace,
        AppleLogLevel.Debug => Microsoft.Extensions.Logging.LogLevel.Debug,
        AppleLogLevel.Information => Microsoft.Extensions.Logging.LogLevel.Information,
        AppleLogLevel.Warning => Microsoft.Extensions.Logging.LogLevel.Warning,
        AppleLogLevel.Error => Microsoft.Extensions.Logging.LogLevel.Error,
        AppleLogLevel.Critical => Microsoft.Extensions.Logging.LogLevel.Critical,
        _ => Microsoft.Extensions.Logging.LogLevel.None
    };

    private void Log(Microsoft.Extensions.Logging.LogLevel level,
                     string message,
                     string? tag)
    {
        if (!this.logger.IsEnabled(level))
            return;

        this.logger.Log(level,
                        "{NativeTag}: {NativeMessage}",
                        tag ?? "AdMobWrapper",
                        message);
    }

    #endregion
}

#endif
