using AMDevIT.Admob.Wrapper.MAUICross.Platforms.iOS.Helpers;
using Microsoft.Extensions.Logging;
using UIKit;

#pragma warning disable IDE0130 // La parola chiave namespace non corrisponde alla struttura di cartelle
namespace AMDevIT.Admob.Wrapper.MAUICross.Services;
#pragma warning restore IDE0130 // La parola chiave namespace non corrisponde alla struttura di cartelle

public class ContextResolverService(ILogger<ContextResolverService> logger)
    : IContextResolverService
{
    #region Properties

    protected ILogger<ContextResolverService> Logger => logger;

    #endregion

    #region Methods

    public object? GetPlatformContext()
    {
        return this.GetViewController();
    }

    public UIViewController? GetViewController()
    {
        UIViewController? currentViewController = ViewControllerHelper.GetTopViewController();

        if (currentViewController == null)
        {
            if (this.Logger.IsEnabled(LogLevel.Warning))
                this.Logger.LogWarning("Could not resolve a view controller from the current context. Returning null");
        }

        return currentViewController;
    }

    #endregion
}
