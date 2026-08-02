#if IOS

using AMDevIT.Admob.Wrapper.iOSNative;
using AMDevIT.Admob.Wrapper.MAUICross.Platforms.iOS.Diagnostics;
using Foundation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using UIKit;
using NativeBannerAdViewSize = AMDevIT.Admob.Wrapper.iOSNative.BannerAdViewSize;

namespace AMDevIT.Admob.Wrapper.MAUICross;

public partial class BannerAdHandler
    : ViewHandler<BannerAd, UIView>
{
    #region Fields

    private BannerAdWrapper? bannerWrapper;
    private AppleLoggerAdapter? loggerAdapter;

    private BannerLoadListener? loadListener;
    private BannerEventListener? eventListener;

    private UIView? currentAdView;
    private nfloat lastAdaptiveWidth;

    #endregion

    #region Methods

    public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
    {
        if (this.VirtualView.AdSize.TryGetFixedSize(out Size fixedSize))
            return fixedSize;

        Size desiredSize = base.GetDesiredSize(widthConstraint, heightConstraint);
        if (desiredSize.Height > 0)
            return desiredSize;

        double width = double.IsFinite(widthConstraint) ? widthConstraint : 0;
        return new Size(width, 50);
    }

    public override void PlatformArrange(Rect frame)
    {
        base.PlatformArrange(frame);

        if (this.VirtualView.AdSize != BannerAdSize.Adaptive || frame.Width <= 0)
            return;

        nfloat width = (nfloat)Math.Floor(frame.Width);
        if (width == this.lastAdaptiveWidth)
            return;

        this.InitializeAdView();
    }

    protected override UIView CreatePlatformView()
    {
        ILoggerFactory loggerFactory = this.MauiContext?.Services.GetRequiredService<ILoggerFactory>()
            ?? throw new InvalidOperationException("The MAUI service provider is not available.");
        this.loggerAdapter = new AppleLoggerAdapter(loggerFactory.CreateLogger<BannerAdHandler>());
        this.bannerWrapper = new BannerAdWrapper(this.loggerAdapter);

        return new BannerView(this.InitializeAdView)
        {
            BackgroundColor = UIColor.Clear
        };
    }

    protected override void DisconnectHandler(UIView platformView)
    {
        this.bannerWrapper?.Destroy();
        this.bannerWrapper?.Dispose();
        this.bannerWrapper = null;
        this.loggerAdapter?.Dispose();
        this.loggerAdapter = null;
        this.loadListener?.Dispose();
        this.loadListener = null;
        this.eventListener?.Dispose();
        this.eventListener = null;
        this.currentAdView?.RemoveFromSuperview();
        this.currentAdView?.Dispose();
        this.currentAdView = null;
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

        nfloat availableWidth = (nfloat)Math.Floor(this.PlatformView.Bounds.Width);
        if (this.VirtualView.AdSize == BannerAdSize.Adaptive && availableWidth <= 0)
            return;

        if (this.currentAdView != null)
        {
            this.currentAdView.RemoveFromSuperview();
            this.currentAdView.Dispose();
            this.currentAdView = null;
        }

        this.loadListener ??= new(this.VirtualView);
        this.eventListener ??= new(this.VirtualView);

        if (this.bannerWrapper == null)
            return;

        NativeBannerAdViewSize adSize = this.MapAdSizeToNative(this.VirtualView.AdSize);
        this.currentAdView = this.bannerWrapper.LoadWithAdUnitId(this.VirtualView.AdUnitId ?? string.Empty,
                                                                  viewController,
                                                                  adSize,
                                                                  availableWidth,
                                                                  this.loadListener,
                                                                  this.eventListener);
        if (this.currentAdView != null)
        {
            this.lastAdaptiveWidth = this.VirtualView.AdSize == BannerAdSize.Adaptive ? availableWidth : 0;
            this.currentAdView.TranslatesAutoresizingMaskIntoConstraints = false;
            this.PlatformView.AddSubview(this.currentAdView);

            NSLayoutConstraint.ActivateConstraints(
            [
                 this.currentAdView.TopAnchor.ConstraintEqualTo(this.PlatformView.TopAnchor),
                 this.currentAdView.BottomAnchor.ConstraintEqualTo(this.PlatformView.BottomAnchor),
                 this.currentAdView.CenterXAnchor.ConstraintEqualTo(this.PlatformView.CenterXAnchor),
                 this.currentAdView.LeadingAnchor.ConstraintGreaterThanOrEqualTo(this.PlatformView.LeadingAnchor),
                 this.currentAdView.TrailingAnchor.ConstraintLessThanOrEqualTo(this.PlatformView.TrailingAnchor),
            ]);
        }        
    }

    partial void UpdateAdUnitId()
    {
        if (this.PlatformView == null)
            return;

        this.InitializeAdView();
    }

    partial void UpdateAdSize()
    {
        if (this.PlatformView == null)
            return;

        this.InitializeAdView();
    }

    private NativeBannerAdViewSize MapAdSizeToNative(BannerAdSize size) => size switch
    {
        BannerAdSize.Adaptive => NativeBannerAdViewSize.Adaptive,
        BannerAdSize.Banner => NativeBannerAdViewSize.Banner,
        BannerAdSize.LargeBanner => NativeBannerAdViewSize.LargeBanner,
        BannerAdSize.MediumRectangle => NativeBannerAdViewSize.MediumRectangle,
        BannerAdSize.FullBanner => NativeBannerAdViewSize.FullBanner,
        BannerAdSize.Leaderboard => NativeBannerAdViewSize.Leaderboard,
        _ => NativeBannerAdViewSize.Banner
    };

    #endregion

    private class BannerLoadListener(BannerAd view)
        : NSObject, IOnAdLoadedListener
    {
        #region Fields

        private readonly BannerAd view = view;

        #endregion

        #region Methods

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

        #endregion
    }

    private class BannerEventListener(BannerAd view)
        : NSObject, IOnAdEventListener
    {
        #region Fields

        private readonly BannerAd view = view;

        #endregion

        #region Methods

        public void OnAdShown() => MainThread.BeginInvokeOnMainThread(() => this.view.RaiseAdLoaded());
        public void OnAdDismissed() => MainThread.BeginInvokeOnMainThread(() => this.view.RaiseAdDismissed());
        public void OnAdClicked() => MainThread.BeginInvokeOnMainThread(() => this.view.RaiseAdClicked());
        public void OnAdImpression() => MainThread.BeginInvokeOnMainThread(() => this.view.RaiseAdImpression());
        public void OnAdFailedToShowWithErrorCode(nint errorCode, string errorMessage) => MainThread.BeginInvokeOnMainThread(() => this.view.RaiseAdFailed((int)errorCode, errorMessage));

        #endregion
    }

    private class BannerView : UIView
    {
        #region Fields

        private readonly Action onReadyAction;
        private bool loaded;

        #endregion

        #region Properties

        public override CoreGraphics.CGSize IntrinsicContentSize
        {
            get
            {
                if (this.Subviews.Length > 0)
                    return this.Subviews[0].IntrinsicContentSize;
                return base.IntrinsicContentSize;
            }
        }

        #endregion

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

        #region Methods

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

        #endregion
    }
}

#endif
