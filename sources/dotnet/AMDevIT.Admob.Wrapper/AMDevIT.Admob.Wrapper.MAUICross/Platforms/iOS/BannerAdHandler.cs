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
    private NSLayoutConstraint[] adViewConstraints = [];
    private string? currentAdUnitId;
    private BannerAdSize currentAdSize;
    private nfloat currentAdWidth;
    private long callbackGeneration;
    private int initializationQueued;
    private int disconnected;
    private bool hasCurrentConfiguration;
    private bool isInitializing;
    private bool initializationRequested;

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
        if (this.hasCurrentConfiguration && width == this.currentAdWidth)
            return;

        this.RequestAdViewInitialization();
    }

    protected override UIView CreatePlatformView()
    {
        ILoggerFactory loggerFactory = this.MauiContext?.Services.GetRequiredService<ILoggerFactory>()
            ?? throw new InvalidOperationException("The MAUI service provider is not available.");

        Volatile.Write(ref this.disconnected, 0);
        this.loggerAdapter = new AppleLoggerAdapter(loggerFactory.CreateLogger<BannerAdHandler>());
        this.bannerWrapper = new BannerAdWrapper(this.loggerAdapter);

        return new BannerView(this.RequestAdViewInitialization)
        {
            BackgroundColor = UIColor.Clear
        };
    }

    protected override void DisconnectHandler(UIView platformView)
    {
        if (!MainThread.IsMainThread)
        {
            MainThread.InvokeOnMainThreadAsync(() => this.DisconnectHandler(platformView))
                      .GetAwaiter()
                      .GetResult();
            return;
        }

        if (Interlocked.Exchange(ref this.disconnected, 1) != 0)
            return;

        Interlocked.Exchange(ref this.initializationQueued, 0);
        this.initializationRequested = false;
        this.callbackGeneration++;
        this.DestroyCurrentBanner();

        this.bannerWrapper?.Dispose();
        this.bannerWrapper = null;
        this.loggerAdapter?.Dispose();
        this.loggerAdapter = null;

        base.DisconnectHandler(platformView);
    }

    private void RequestAdViewInitialization()
    {
        if (Volatile.Read(ref this.disconnected) != 0)
            return;

        if (MainThread.IsMainThread)
        {
            this.InitializeAdView();
            return;
        }

        if (Interlocked.Exchange(ref this.initializationQueued, 1) != 0)
            return;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            Interlocked.Exchange(ref this.initializationQueued, 0);
            this.InitializeAdView();
        });
    }

    private void InitializeAdView()
    {
        if (Volatile.Read(ref this.disconnected) != 0)
            return;

        if (this.isInitializing)
        {
            this.initializationRequested = true;
            return;
        }

        this.isInitializing = true;

        try
        {
            do
            {
                this.initializationRequested = false;
                this.InitializeAdViewCore();
            }
            while (this.initializationRequested && Volatile.Read(ref this.disconnected) == 0);
        }
        finally
        {
            this.isInitializing = false;
        }
    }

    private void InitializeAdViewCore()
    {
        BannerAdSize adSize;
        BannerAdWrapper bannerWrapper;
        BannerEventListener eventListener;
        BannerLoadListener loadListener;
        NativeBannerAdViewSize nativeAdSize;
        UIView? adView = null;
        UIViewController? viewController;
        string adUnitId;
        nfloat availableWidth;
        nfloat effectiveWidth;
        long generation;

        if (this.bannerWrapper == null || Volatile.Read(ref this.disconnected) != 0)
            return;

        bannerWrapper = this.bannerWrapper;
        adUnitId = this.VirtualView.AdUnitId ?? string.Empty;
        adSize = this.VirtualView.AdSize;
        availableWidth = (nfloat)Math.Floor(this.PlatformView.Bounds.Width);
        effectiveWidth = adSize == BannerAdSize.Adaptive ? availableWidth : 0;

        if (string.IsNullOrWhiteSpace(adUnitId))
        {
            this.callbackGeneration++;
            this.DestroyCurrentBanner();
            return;
        }

        if (adSize == BannerAdSize.Adaptive && effectiveWidth <= 0)
            return;

        if (this.hasCurrentConfiguration &&
            string.Equals(this.currentAdUnitId, adUnitId, StringComparison.Ordinal) &&
            this.currentAdSize == adSize &&
            this.currentAdWidth == effectiveWidth)
        {
            return;
        }

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

        this.callbackGeneration++;
        generation = this.callbackGeneration;
        this.DestroyCurrentBanner();
        loadListener = new(this, generation);
        eventListener = new(this, generation);
        nativeAdSize = this.MapAdSizeToNative(adSize);
        this.loadListener = loadListener;
        this.eventListener = eventListener;

        try
        {
            adView = bannerWrapper.LoadWithAdUnitId(adUnitId,
                                                    viewController,
                                                    nativeAdSize,
                                                    availableWidth,
                                                    loadListener,
                                                    eventListener);
            this.currentAdView = adView;
            adView.TranslatesAutoresizingMaskIntoConstraints = false;
            this.PlatformView.AddSubview(adView);
            this.adViewConstraints =
            [
                adView.TopAnchor.ConstraintEqualTo(this.PlatformView.TopAnchor),
                adView.BottomAnchor.ConstraintEqualTo(this.PlatformView.BottomAnchor),
                adView.CenterXAnchor.ConstraintEqualTo(this.PlatformView.CenterXAnchor),
                adView.LeadingAnchor.ConstraintGreaterThanOrEqualTo(this.PlatformView.LeadingAnchor),
                adView.TrailingAnchor.ConstraintLessThanOrEqualTo(this.PlatformView.TrailingAnchor),
            ];
            NSLayoutConstraint.ActivateConstraints(this.adViewConstraints);

            this.currentAdUnitId = adUnitId;
            this.currentAdSize = adSize;
            this.currentAdWidth = effectiveWidth;
            this.hasCurrentConfiguration = true;
        }
        catch
        {
            this.DestroyCurrentBanner();
            throw;
        }
    }

    private void DestroyCurrentBanner()
    {
        NSLayoutConstraint[] constraints = this.adViewConstraints;
        UIView? adView = this.currentAdView;
        BannerLoadListener? loadListener = this.loadListener;
        BannerEventListener? eventListener = this.eventListener;

        this.adViewConstraints = [];
        this.currentAdView = null;
        this.loadListener = null;
        this.eventListener = null;
        this.ResetCurrentConfiguration();

        if (constraints.Length > 0)
        {
            NSLayoutConstraint.DeactivateConstraints(constraints);
            foreach (NSLayoutConstraint constraint in constraints)
                constraint.Dispose();
        }

        this.bannerWrapper?.Destroy();
        adView?.RemoveFromSuperview();
        adView?.Dispose();
        loadListener?.Dispose();
        eventListener?.Dispose();
    }

    private void ResetCurrentConfiguration()
    {
        this.currentAdUnitId = null;
        this.currentAdSize = default;
        this.currentAdWidth = 0;
        this.hasCurrentConfiguration = false;
    }

    private bool IsCurrentCallback(long generation) =>
        Volatile.Read(ref this.disconnected) == 0 &&
        generation == this.callbackGeneration &&
        this.currentAdView != null;

    private void OnAdLoaded(long generation)
    {
        if (!this.IsCurrentCallback(generation))
            return;

        this.VirtualView.InvalidateMeasure();
        this.VirtualView.RaiseAdLoaded();
    }

    private void OnAdFailed(long generation, int errorCode, string errorMessage)
    {
        if (this.IsCurrentCallback(generation))
            this.VirtualView.RaiseAdFailed(errorCode, errorMessage);
    }

    private void OnAdShown(long generation)
    {
        if (this.IsCurrentCallback(generation))
            this.VirtualView.RaiseAdLoaded();
    }

    private void OnAdDismissed(long generation)
    {
        if (this.IsCurrentCallback(generation))
            this.VirtualView.RaiseAdDismissed();
    }

    private void OnAdClicked(long generation)
    {
        if (this.IsCurrentCallback(generation))
            this.VirtualView.RaiseAdClicked();
    }

    private void OnAdImpression(long generation)
    {
        if (this.IsCurrentCallback(generation))
            this.VirtualView.RaiseAdImpression();
    }

    partial void UpdateAdUnitId()
    {
        if (this.PlatformView == null)
            return;

        this.RequestAdViewInitialization();
    }

    partial void UpdateAdSize()
    {
        if (this.PlatformView == null)
            return;

        this.RequestAdViewInitialization();
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

    private class BannerLoadListener(BannerAdHandler handler,
                                     long generation)
        : NSObject, IOnAdLoadedListener
    {
        #region Fields

        private readonly WeakReference<BannerAdHandler> handlerReference = new(handler);
        private readonly long generation = generation;

        #endregion

        #region Methods

        public void OnAdLoaded()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (this.handlerReference.TryGetTarget(out BannerAdHandler? handler))
                    handler.OnAdLoaded(this.generation);
            });
        }

        public void OnAdFailedToLoadWithErrorCode(nint errorCode, string errorMessage)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (this.handlerReference.TryGetTarget(out BannerAdHandler? handler))
                    handler.OnAdFailed(this.generation, (int)errorCode, errorMessage);
            });
        }

        #endregion
    }

    private class BannerEventListener(BannerAdHandler handler,
                                      long generation)
        : NSObject, IOnAdEventListener
    {
        #region Fields

        private readonly WeakReference<BannerAdHandler> handlerReference = new(handler);
        private readonly long generation = generation;

        #endregion

        #region Methods

        public void OnAdShown() => this.Dispatch(handler => handler.OnAdShown(this.generation));
        public void OnAdDismissed() => this.Dispatch(handler => handler.OnAdDismissed(this.generation));
        public void OnAdClicked() => this.Dispatch(handler => handler.OnAdClicked(this.generation));
        public void OnAdImpression() => this.Dispatch(handler => handler.OnAdImpression(this.generation));
        public void OnAdFailedToShowWithErrorCode(nint errorCode, string errorMessage) =>
            this.Dispatch(handler => handler.OnAdFailed(this.generation, (int)errorCode, errorMessage));

        private void Dispatch(Action<BannerAdHandler> action)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (this.handlerReference.TryGetTarget(out BannerAdHandler? handler))
                    action(handler);
            });
        }

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
