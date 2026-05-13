using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace AMDevIT.Admob.Wrapper.MAUITestApp.ViewModels;

public class ViewModelBase(ILogger logger)
    : ObservableObject
{
    #region Properties

    protected ILogger Logger => logger;

    #endregion

    #region Methods

    public virtual void RegisterEvents()
    {
        if (this.Logger.IsEnabled(LogLevel.Debug))
            this.Logger.LogDebug("Register events for {ViewModelName}", this.GetType().Name);
    }

    public virtual void UnregisterEvents() 
    { 
        if (this.Logger.IsEnabled(LogLevel.Debug))
            this.Logger.LogDebug("Unregister events for {ViewModelName}", this.GetType().Name);
    }

    #endregion
}
