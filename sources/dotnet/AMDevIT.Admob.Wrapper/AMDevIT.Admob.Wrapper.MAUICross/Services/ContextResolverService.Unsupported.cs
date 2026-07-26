#if WINDOWS || MACCATALYST

using Microsoft.Extensions.Logging;

namespace AMDevIT.Admob.Wrapper.MAUICross.Services;

public class ContextResolverService(ILogger<ContextResolverService> logger)
    : IContextResolverService
{
    #region Properties

    protected ILogger<ContextResolverService> Logger => logger;

    #endregion

    #region Methods

    public object? GetPlatformContext()
    {
        if (this.Logger.IsEnabled(LogLevel.Warning))
            this.Logger.LogWarning("AdMob isn't supported on this platform. Returning a null platform context.");

        return null;
    }

    #endregion
}

#endif
