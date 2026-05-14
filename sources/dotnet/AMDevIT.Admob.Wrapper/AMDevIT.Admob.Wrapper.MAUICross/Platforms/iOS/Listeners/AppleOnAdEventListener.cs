using AMDevIT.Admob.Wrapper.iOSNative;
using Foundation;

namespace AMDevIT.Admob.Wrapper.MAUICross.Platforms.iOS.Listeners;

internal class AppleOnAdEventListener
    : NSObject, IOnAdEventListener
{
    #region Events

    public event EventHandler? AdClicked;
    public event EventHandler? AdDismissed;
    public event EventHandler<AdFailedToShowArgs>? AdFailedToShow;
    public event EventHandler? AdImpression;
    public event EventHandler? AdShown;

    #endregion

    #region Methods

    public void OnAdShown()
    {
        this.AdShown?.Invoke(this, EventArgs.Empty);
    }

    public void OnAdDismissed()
    {
        this.AdDismissed?.Invoke(this, EventArgs.Empty);
    }

    public void OnAdClicked()
    {
        this.AdClicked?.Invoke(this, EventArgs.Empty);
    }

    public void OnAdImpression()
    {
        this.AdImpression?.Invoke(this, EventArgs.Empty);
    }

    public void OnAdFailedToShowWithErrorCode(nint errorCode, string errorMessage)
    {
        AdFailedToShowArgs args = new(errorCode, errorMessage);
        this.AdFailedToShow?.Invoke(this, args);
    }

    #endregion
}
