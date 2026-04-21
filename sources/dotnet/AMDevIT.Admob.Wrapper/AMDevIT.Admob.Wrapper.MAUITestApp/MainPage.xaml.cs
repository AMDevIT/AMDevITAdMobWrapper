using Microsoft.Extensions.Logging;

namespace AMDevIT.Admob.Wrapper.MAUITestApp
{
    public partial class MainPage : ContentPage
    {
        #region Properties

        protected ILogger<MainPage> Logger
        { 
            get; 
        }

        #endregion

        public MainPage(ILogger<MainPage> logger)
        {
            InitializeComponent();
            this.Logger = logger;
        }

        #region Event Handlers
        private void BannerAd_AdLoaded(object sender, EventArgs e)
        {
           Logger.LogDebug("Banner loaded");
        }

        private void BannerAd_AdFailed(object sender, MAUICross.BannerAdFailedEventArgs e)
        {
            Logger.LogDebug($"Banner failed: [{e.ErrorCode}] {e.ErrorMessage}");
        }

        #endregion
    }
}
