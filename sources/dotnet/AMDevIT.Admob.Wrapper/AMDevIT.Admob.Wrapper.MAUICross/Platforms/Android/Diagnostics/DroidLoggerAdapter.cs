#if ANDROID

using AMDevIT.Admob.Wrapper.Diagnostics;
using Android.Runtime;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NativeLogLevel = AMDevIT.Admob.Wrapper.Diagnostics.LogLevel;

namespace AMDevIT.Admob.Wrapper.MAUICross.Platforms.Android.Diagnostics;

internal class DroidLoggerAdapter : Java.Lang.Object, IDroidLogger
{
    #region Fields

    private ILogger logger;

    #endregion

    #region .ctor

    public DroidLoggerAdapter()
    {
        this.logger = NullLogger.Instance;
    }

    public DroidLoggerAdapter(ILogger logger)
    {
        this.logger = logger ?? NullLogger.Instance;
    }

    protected DroidLoggerAdapter(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
        this.logger = NullLogger.Instance;
    }

    #endregion

    #region Methods

    public bool IsEnabled(NativeLogLevel level)
    {
        try
        {
            return this.logger.IsEnabled(MapLogLevel(level));
        }
        catch
        {
            return false;
        }
    }

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

    protected override void Dispose(bool disposing)
    {
        this.logger = NullLogger.Instance;
        base.Dispose(disposing);
    }

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
        try
        {
            if (!this.logger.IsEnabled(level))
                return;

            this.logger.Log(level,
                            "{NativeTag}: {NativeMessage}",
                            tag ?? "AdMobWrapper",
                            message);
        }
        catch
        {
            // Diagnostics must never terminate a Java callback thread.
        }
    }

    #endregion
}

#endif
