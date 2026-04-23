#if ANDROID

using AMDevIT.Admob.Wrapper.Listeners;

namespace AMDevIT.Admob.Wrapper.MAUICross.Platforms.Android.Listeners;

public class OnAdLoadedListener
    : Java.Lang.Object, IOnAdLoadedListener
{
    #region Events

    public event EventHandler? AdLoaded;
    public event EventHandler<AdFailedToLoadEventArgs>? AdFailedToLoad;

    #endregion

    #region Methods

    public void OnAdFailedToLoad(int errorCode, string errorMessage)
    {
        AdFailedToLoadEventArgs args = new(errorCode, errorMessage);
        this.AdFailedToLoad?.Invoke(this, args);
    }

    public void OnAdLoaded()
    {
        this.AdLoaded?.Invoke(this, EventArgs.Empty);
    }

    #endregion
}

#endif