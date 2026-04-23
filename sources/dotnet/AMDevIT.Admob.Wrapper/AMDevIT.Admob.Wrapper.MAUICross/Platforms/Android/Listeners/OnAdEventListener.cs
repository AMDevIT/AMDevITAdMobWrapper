using AMDevIT.Admob.Wrapper.Listeners;

namespace AMDevIT.Admob.Wrapper.MAUICross.Platforms.Android.Listeners
{
    public class OnAdEventListener
        : Java.Lang.Object, IOnAdEventListener
    {
        #region Events

        public event EventHandler? AdClicked;
        public event EventHandler? AdDismissed;
        public event EventHandler<AdFailedToShowEventArgs>? AdFailedToShow;
        public event EventHandler? AdImpression;
        public event EventHandler? AdShown;

        #endregion

        #region Methods

        public void OnAdClicked()
        {
            this.AdClicked?.Invoke(this, EventArgs.Empty);
        }

        public void OnAdDismissed()
        {
            this.AdDismissed?.Invoke(this, EventArgs.Empty);
        }

        public void OnAdFailedToShow(int errorCode, string errorMessage)
        {
            AdFailedToShowEventArgs args = new(errorCode, errorMessage);
            this.AdFailedToShow?.Invoke(this, args);
        }

        public void OnAdImpression()
        {
            this.AdImpression?.Invoke(this, EventArgs.Empty);
        }

        public void OnAdShown()
        {
            this.AdShown?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
