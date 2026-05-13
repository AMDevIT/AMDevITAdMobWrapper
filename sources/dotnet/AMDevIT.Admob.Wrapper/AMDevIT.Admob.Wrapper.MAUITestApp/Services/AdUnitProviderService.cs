using Microsoft.Extensions.Logging;

namespace AMDevIT.Admob.Wrapper.MAUITestApp.Services
{
    public partial class AdUnitProviderService(ILogger<AdUnitProviderService> logger) 
        : IAdUnitProviderService
    {
        #region Consts

        private const string BannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";
        private const string InterstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
        private const string RewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";
        private const string AppOpenAdUnitId = "ca-app-pub-3940256099942544/9257395921";


        #endregion

        #region Properties

        protected ILogger<AdUnitProviderService> Logger => logger;

        #endregion

        #region Methods

        public string GetBannerAdUnitId()
        {
            if (this.Logger.IsEnabled(LogLevel.Debug))
                this.Logger.LogDebug("Retrieving banner ad unit id: {AdUnitId}", BannerAdUnitId);

            return BannerAdUnitId;
        }

        public string GetInterstitialAdUnitId()
        {
            if (this.Logger.IsEnabled(LogLevel.Debug))
                this.Logger.LogDebug("Retrieving interstitial ad unit id: {AdUnitId}", InterstitialAdUnitId);

            return InterstitialAdUnitId;
        }

        public string GetRewardedAdUnitId()
        {
            if (this.Logger.IsEnabled(LogLevel.Debug))
                this.Logger.LogDebug("Retrieving rewarded ad unit id: {AdUnitId}", RewardedAdUnitId);

            return RewardedAdUnitId;
        }

        public string GetAppOpenAdUnitId()
        {
            if (this.Logger.IsEnabled(LogLevel.Debug))
                this.Logger.LogDebug("Retrieving app open ad unit id: {AdUnitId}", AppOpenAdUnitId);

            return AppOpenAdUnitId;
        }

        #endregion
    }
}
