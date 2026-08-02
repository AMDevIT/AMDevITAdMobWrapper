#if ANDROID

using AMDevIT.Admob.Wrapper.Diagnostics;
using Microsoft.Extensions.Logging;
using NativeLogLevel = AMDevIT.Admob.Wrapper.Diagnostics.LogLevel;

namespace AMDevIT.Admob.Wrapper.MAUICross.Platforms.Android.Diagnostics;

internal sealed class DroidLoggerAdapter : Java.Lang.Object, IDroidLogger
{
    #region Fields

    private readonly ILogger logger;

    #endregion

    #region .ctor

    public DroidLoggerAdapter(ILogger logger)
    {
        this.logger = logger;
    }

    #endregion

    #region Methods

    public bool IsEnabled(NativeLogLevel level) =>
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

    private static Microsoft.Extensions.Logging.LogLevel MapLogLevel(NativeLogLevel level)
    {
        if (NativeLogLevel.Trace?.Equals(level) == true)
            return Microsoft.Extensions.Logging.LogLevel.Trace;
        if (NativeLogLevel.Debug?.Equals(level) == true)
            return Microsoft.Extensions.Logging.LogLevel.Debug;
        if (NativeLogLevel.Information?.Equals(level) == true)
            return Microsoft.Extensions.Logging.LogLevel.Information;
        if (NativeLogLevel.Warning?.Equals(level) == true)
            return Microsoft.Extensions.Logging.LogLevel.Warning;
        if (NativeLogLevel.Error?.Equals(level) == true)
            return Microsoft.Extensions.Logging.LogLevel.Error;
        if (NativeLogLevel.Critical?.Equals(level) == true)
            return Microsoft.Extensions.Logging.LogLevel.Critical;

        return Microsoft.Extensions.Logging.LogLevel.None;
    }

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
