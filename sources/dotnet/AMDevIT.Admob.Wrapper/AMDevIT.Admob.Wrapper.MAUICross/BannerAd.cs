namespace AMDevIT.Admob.Wrapper.MAUICross
{
    // All the code in this file is included in all platforms.
    public class BannerAd
        : View
    {
        #region Events

        public event EventHandler? AdLoaded;
        public event EventHandler<BannerAdFailedEventArgs>? AdFailed;
        public event EventHandler? AdClicked;
        public event EventHandler? AdImpression;

        #endregion

        #region Depency Properties

        public static readonly BindableProperty AdUnitIdProperty =
           BindableProperty.Create(nameof(AdUnitId), typeof(string), typeof(BannerAd), null);

        public static readonly BindableProperty AdSizeProperty =
            BindableProperty.Create(nameof(AdSize), typeof(BannerAdSize), typeof(BannerAd), BannerAdSize.Adaptive);

        #endregion

        #region Properties

        public string? AdUnitId
        {
            get => (string?)GetValue(AdUnitIdProperty);
            set => SetValue(AdUnitIdProperty, value);
        }

        public BannerAdSize AdSize
        {
            get => (BannerAdSize)GetValue(AdSizeProperty);
            set => SetValue(AdSizeProperty, value);
        }

        #endregion

        #region Methods

        internal void RaiseAdLoaded() =>
            this.AdLoaded?.Invoke(this, EventArgs.Empty);

        internal void RaiseAdFailed(int errorCode, string errorMessage) =>
            this.AdFailed?.Invoke(this, new BannerAdFailedEventArgs(errorCode, errorMessage));

        internal void RaiseAdClicked() =>
            this.AdClicked?.Invoke(this, EventArgs.Empty);

        internal void RaiseAdImpression() =>
            this.AdImpression?.Invoke(this, EventArgs.Empty);

        #endregion
    }
}
