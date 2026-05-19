#if ANDROID

using AMDevIT.Admob.Wrapper.Ads;
using AMDevIT.Admob.Wrapper.Listeners;
using Microsoft.Maui.Handlers;
using AndroidView = Android.Views.View;

namespace AMDevIT.Admob.Wrapper.MAUICross;

public partial class BannerAdHandler 
    : ViewHandler<BannerAd, AndroidView>
{

    #region Fields

    private BannerAdWrapper? bannerWrapper;

    #endregion

    #region Methods

    protected override AndroidView CreatePlatformView()
    {
        this.bannerWrapper = new BannerAdWrapper(this.Context);
        var adUnitId = this.VirtualView.AdUnitId ?? string.Empty;
        var adSize = this.MapAdSizeToNative(this.VirtualView.AdSize);

        AndroidView bannerView = this.bannerWrapper.Load(adUnitId,
                                                         adSize,
                                                         new BannerLoadListener(this.VirtualView),
                                                         new BannerEventListener(this.VirtualView));
        return bannerView;
    }

    protected override void DisconnectHandler(AndroidView platformView)
    {
        this.bannerWrapper?.Destroy();
        base.DisconnectHandler(platformView);
    }

    partial void UpdateAdUnitId()
    {
        if (this.PlatformView == null) return;
        this.bannerWrapper?.Destroy();
        var adSize = this.MapAdSizeToNative(this.VirtualView.AdSize);
        this.bannerWrapper?.Load(this.VirtualView.AdUnitId ?? string.Empty,
                                 adSize,
                                 new BannerLoadListener(this.VirtualView),
                                 new BannerEventListener(this.VirtualView));
    }

    // Manager in CreatePlatformView
    partial void UpdateAdSize() 
    { 
    } 

    private BannerAdViewSize MapAdSizeToNative(BannerAdSize size) => size switch
    {   
        BannerAdSize.Banner => BannerAdViewSize.Banner,
        BannerAdSize.LargeBanner => BannerAdViewSize.LargeBanner,
        BannerAdSize.MediumRectangle => BannerAdViewSize.MediumRectangle,
        BannerAdSize.FullBanner => BannerAdViewSize.FullBanner,
        BannerAdSize.Leaderboard => BannerAdViewSize.Leaderboard,
        BannerAdSize.Adaptive => BannerAdViewSize.Adaptive,
        _ => BannerAdViewSize.Banner
    };

    #region Nested classes

    private class BannerLoadListener(BannerAd view)
                : Java.Lang.Object, IOnAdLoadedListener
    {
        private readonly BannerAd view = view;

        public void OnAdLoaded()
        {
            MainThread.BeginInvokeOnMainThread(() => this.view.RaiseAdLoaded());
        }

        public void OnAdFailedToLoad(int errorCode, string errorMessage)
        {
            MainThread.BeginInvokeOnMainThread(() => this.view.RaiseAdFailed(errorCode, errorMessage));
        }
    }

    private class BannerEventListener(BannerAd view)
                : Java.Lang.Object, IOnAdEventListener
    {
        private readonly BannerAd view = view;

        public void OnAdShown() 
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                this.view.InvalidateMeasure();
                this.view.RaiseAdLoaded();
            });            
        }

        public void OnAdDismissed() 
        {            
            MainThread.BeginInvokeOnMainThread(() => this.view.RaiseAdDismissed());
        }

        public void OnAdClicked()
        {
            MainThread.BeginInvokeOnMainThread(() => this.view.RaiseAdClicked());
        }

        public void OnAdImpression()
        {
            MainThread.BeginInvokeOnMainThread(() => this.view.RaiseAdImpression());
        }                

        public void OnAdFailedToShow(int errorCode, string errorMessage) 
        {
            MainThread.BeginInvokeOnMainThread(() => this.view.RaiseAdFailed(errorCode, errorMessage));
        }
    }

    #endregion

    #endregion
}

#endif