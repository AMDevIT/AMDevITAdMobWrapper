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

    private BannerLoadListener? loadListener;
    private BannerEventListener? eventListener;

    private UIView? currentAdView;

    protected override UIView CreatePlatformView()
    {
        UIView view;

        view = new BannerView(this.InitializeAdView);
        view.BackgroundColor = UIColor.Clear;
        return view;
    }

    protected override void ConnectHandler(UIView platformView)
    {
        base.ConnectHandler(platformView);

        // platformView.TranslatesAutoresizingMaskIntoConstraints = false;
        // if (platformView.Superview != null)
        // {
        //     NSLayoutConstraint.ActivateConstraints(
        //     [
        //          platformView.TopAnchor.ConstraintEqualTo(platformView.Superview.TopAnchor),
        //          platformView.BottomAnchor.ConstraintEqualTo(platformView.Superview.BottomAnchor),
        //          platformView.LeadingAnchor.ConstraintEqualTo(platformView.Superview.LeadingAnchor),
        //          platformView.TrailingAnchor.ConstraintEqualTo(platformView.Superview.TrailingAnchor),
        //     ]);
        // }
    }

    protected override void DisconnectHandler(UIView platformView)
    {
        this.bannerWrapper?.Destroy();
        base.DisconnectHandler(platformView);
    }

    private void InitializeAdView()
    {
        UIViewController? viewController;

        try
        {
            viewController = Platform.GetCurrentUIViewController();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to get current UIViewController: {ex}");
            viewController = null;
        }

        if (viewController == null)
            return;

        if (this.currentAdView != null)
        {
            this.currentAdView.RemoveFromSuperview();
            this.currentAdView.Dispose();
            this.currentAdView = null;
        }

        this.loadListener ??= new(this.VirtualView);
        this.eventListener ??= new(this.VirtualView);

        this.bannerWrapper ??= new();

        this.currentAdView = this.bannerWrapper.LoadWithAdUnitId(this.VirtualView.AdUnitId ?? string.Empty,
                                                                  viewController,
                                                                  this.loadListener,
                                                                  this.eventListener);
        if (this.currentAdView != null)
        {
            this.currentAdView.TranslatesAutoresizingMaskIntoConstraints = false;
            this.PlatformView.AddSubview(this.currentAdView);

            // Center the ad view within the platform view
            NSLayoutConstraint.ActivateConstraints(
            [
                 this.currentAdView.TopAnchor.ConstraintEqualTo(this.PlatformView.TopAnchor),
                 this.currentAdView.BottomAnchor.ConstraintEqualTo(this.PlatformView.BottomAnchor),
                 this.currentAdView.LeadingAnchor.ConstraintEqualTo(this.PlatformView.LeadingAnchor),
                 this.currentAdView.TrailingAnchor.ConstraintEqualTo(this.PlatformView.TrailingAnchor),
            ]);
        }
    }

    partial void UpdateAdUnitId()
    {
        if (this.PlatformView == null)
            return;

        // if (this.bannerWrapper != null)
        // {
        //     this.bannerWrapper?.Destroy();
        //     this.bannerWrapper?.Dispose();
        //     this.bannerWrapper = null;
        // }

        this.InitializeAdView();
    }

    partial void UpdateAdSize() { }

    #region Nested classes

    #region Listeners

    private class BannerLoadListener(BannerAd view)
        : NSObject, IOnAdLoadedListener
    {
        private readonly BannerAd view = view;

        public void OnAdLoaded()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                this.view.InvalidateMeasure();
                this.view.RaiseAdLoaded();
            });
        }

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

    #region Views 

    private class BannerView : UIView
    {
        private readonly Action onReadyAction;
        private bool loaded = false;

        #region .ctor

        public BannerView(Action onReadyAction)
            : base()
        {
            this.onReadyAction = onReadyAction;
        }

        public BannerView(Foundation.NSCoder coder)
            : base(coder)
        {
            throw new NotSupportedException("This constructor is not supported for BannerView.");
        }

        #endregion

        public override void MovedToWindow()
        {
            base.MovedToWindow();

            if (this.Window == null || this.loaded)
                return;

            this.loaded = true;
            this.onReadyAction();
        }

        public override CoreGraphics.CGSize SizeThatFits(CoreGraphics.CGSize size)
        {
            if (this.Subviews.Length > 0)
                return this.Subviews[0].SizeThatFits(size);
            return base.SizeThatFits(size);
        }

        public override CoreGraphics.CGSize IntrinsicContentSize
        {
            get
            {
                if (this.Subviews.Length > 0)
                    return this.Subviews[0].IntrinsicContentSize;
                return base.IntrinsicContentSize;
            }
        }
    }

    #endregion

    #endregion
}

#endif