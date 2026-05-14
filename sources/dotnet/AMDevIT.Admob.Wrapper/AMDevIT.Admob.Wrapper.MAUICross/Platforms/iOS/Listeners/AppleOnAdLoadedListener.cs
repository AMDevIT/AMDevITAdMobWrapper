using AMDevIT.Admob.Wrapper.iOSNative;
using Foundation;

namespace AMDevIT.Admob.Wrapper.MAUICross.Platforms.iOS.Listeners;

internal class AppleOnAdLoadedListener
    : NSObject, IOnAdLoadedListener        
{
    #region Events

    public event EventHandler? AdLoaded;
    public event EventHandler<AdFailedToLoadArgs>? AdFailedToLoad;

    #endregion

    #region Methods

    public void OnAdLoaded()
    {
        this.AdLoaded?.Invoke(this, EventArgs.Empty);
    }

    public void OnAdFailedToLoadWithErrorCode(nint errorCode, string errorMessage)
    {
        AdFailedToLoadArgs args = new(errorCode, errorMessage);
        this.AdFailedToLoad?.Invoke(this, args);
    }

    #endregion
}
