#if IOS

using AMDevIT.Admob.Wrapper.iOSNative;
using Foundation;
using Microsoft.Maui.Handlers;
using UIKit;

namespace AMDevIT.Admob.Wrapper.MAUICross;

public partial class BannerAdHandler 
    : ViewHandler<BannerAd, UIView>
{
    private BannerAdWrapper? bannerWrapper;

    protected override UIView CreatePlatformView()
    {
        this.bannerWrapper = new ();

        string adUnitId = this.VirtualView.AdUnitId ?? string.Empty;
        UIViewController? viewController = Platform.GetCurrentUIViewController();

        return this.bannerWrapper.LoadWithAdUnitId(adUnitId,
                                                   viewController!,
                                                   new BannerLoadListener(this.VirtualView),
                                                   new BannerEventListener(this.VirtualView)
        );
    }

    protected override void DisconnectHandler(UIView platformView)
    {
        this.bannerWrapper?.Destroy();
        base.DisconnectHandler(platformView);
    }

    partial void UpdateAdUnitId()
    {
        if (this.PlatformView == null) return;
        this.bannerWrapper?.Destroy();
        UIViewController? viewController = Platform.GetCurrentUIViewController();
        this.bannerWrapper?.LoadWithAdUnitId(this.VirtualView.AdUnitId ?? string.Empty,
                                             viewController!,
                                             new BannerLoadListener(this.VirtualView),
                                             new BannerEventListener(this.VirtualView));
    }

    partial void UpdateAdSize() { }

    #region Nested classes

    private class BannerLoadListener(BannerAd view) 
        : NSObject, IOnAdLoadedListener
    {
        private readonly BannerAd view = view;

        public void OnAdLoaded() => MainThread.BeginInvokeOnMainThread(() => this.view.RaiseAdLoaded());

        public void OnAdFailedToLoadWithErrorCode(nint errorCode, string errorMessage)
        {
            MainThread.BeginInvokeOnMainThread(() => this.view.RaiseAdFailed((int)errorCode, errorMessage));
        }
    }

    private class BannerEventListener(BannerAd view) 
        : NSObject, IOnAdEventListener
    {
        private readonly BannerAd view = view;

        public void OnAdShown() { }
        public void OnAdDismissed() { }
        public void OnAdClicked() => MainThread.BeginInvokeOnMainThread(() => this.view.RaiseAdClicked());
        public void OnAdImpression() => MainThread.BeginInvokeOnMainThread(() => this.view.RaiseAdImpression());
        public void OnAdFailedToShowWithErrorCode(nint errorCode, string errorMessage) { }
    }

    #endregion
}

#endif