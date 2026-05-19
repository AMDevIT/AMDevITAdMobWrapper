using Android.App;
using Android.Content;
using Microsoft.Extensions.Logging;

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

    /// <summary>
    /// Resolve current MAUI activity.
    /// </summary>
    /// <returns>A valid Android context if available; otherwise, null.</returns>
    public Context? GetContext()
    {
        Activity? currentActivity = Platform.CurrentActivity;

        if (currentActivity == null)
        {
            if (this.Logger.IsEnabled(LogLevel.Warning))
                this.Logger.LogWarning("Cannot resolve current activity. Make sure that the Android project is properly configured and " +
                                       "that the app has been launched at least once. Returning null.");
        }
        return currentActivity;
    }

    /// <summary>
    /// Platform independent method to resolve current MAUI activity. It returns the same value as the platform-specific GetContext method but is defined in the shared code to be 
    /// used by services without referencing platform-specific assemblies.
    /// </summary>
    /// <returns>A valid Android context if available; otherwise, null.</returns>
    public object? GetPlatformContext()
    {
        return this.GetContext();
    }

    #endregion
}
