#if ANDROID

using AMDevIT.Admob.Wrapper.Ads;
using AMDevIT.Admob.Wrapper.Listeners;
using AMDevIT.Admob.Wrapper.MAUICross.Platforms.Android.Diagnostics;
using Android.Content;
using Android.Views;
using Android.Widget;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using AndroidView = Android.Views.View;

namespace AMDevIT.Admob.Wrapper.MAUICross;

public partial class BannerAdHandler 
    : ViewHandler<BannerAd, AndroidView>
{

    #region Fields

    private BannerAdWrapper? bannerWrapper;
    private DroidLoggerAdapter? loggerAdapter;
    private int lastAdaptiveWidth;

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

    protected override AndroidView CreatePlatformView()
    {
        ILoggerFactory loggerFactory = this.MauiContext?.Services.GetRequiredService<ILoggerFactory>()
            ?? throw new InvalidOperationException("The MAUI service provider is not available.");
        this.loggerAdapter = new DroidLoggerAdapter(loggerFactory.CreateLogger<BannerAdHandler>());
        this.bannerWrapper = new BannerAdWrapper(this.Context, this.loggerAdapter);
        BannerContainer container = new(this.Context, this.OnContainerWidthChanged);

        if (this.VirtualView.AdSize != BannerAdSize.Adaptive)
            this.LoadBanner(container, null);

        return container;
    }

    protected override void DisconnectHandler(AndroidView platformView)
    {
        this.bannerWrapper?.Destroy();
        this.bannerWrapper?.Dispose();
        this.bannerWrapper = null;
        this.loggerAdapter?.Dispose();
        this.loggerAdapter = null;
        if (platformView is BannerContainer container)
        {
            container.WidthChanged = null;
            container.RemoveAllViews();
        }

        base.DisconnectHandler(platformView);
    }

    partial void UpdateAdUnitId()
    {
        if (this.PlatformView is not BannerContainer container)
            return;

        this.LoadBanner(container, this.lastAdaptiveWidth);
    }

    partial void UpdateAdSize()
    {
        if (this.PlatformView is not BannerContainer container)
            return;

        this.lastAdaptiveWidth = 0;
        this.LoadBanner(container, this.GetAvailableWidth(container));
    }

    private int GetAvailableWidth(BannerContainer container)
    {
        if (container.Width <= 0)
            return 0;

        float density = this.Context.Resources?.DisplayMetrics?.Density ?? 1;
        return Math.Max(1, (int)Math.Floor(container.Width / density));
    }

    private void LoadBanner(BannerContainer container, int? adaptiveWidth)
    {
        if (this.bannerWrapper == null)
            return;

        string adUnitId = this.VirtualView.AdUnitId ?? string.Empty;
        BannerLoadListener loadListener = new(this.VirtualView);
        BannerEventListener eventListener = new(this.VirtualView);
        AndroidView bannerView;

        if (this.VirtualView.AdSize == BannerAdSize.Adaptive)
        {
            if (adaptiveWidth is not > 0)
                return;

            this.lastAdaptiveWidth = adaptiveWidth.Value;
            bannerView = this.bannerWrapper.LoadAdaptive(adUnitId,
                                                         adaptiveWidth.Value,
                                                         loadListener,
                                                         eventListener);
        }
        else
        {
            BannerAdViewSize adSize = this.MapAdSizeToNative(this.VirtualView.AdSize);
            this.lastAdaptiveWidth = 0;
            bannerView = this.bannerWrapper.Load(adUnitId,
                                                 adSize,
                                                 loadListener,
                                                 eventListener);
        }

        container.RemoveAllViews();
        FrameLayout.LayoutParams layoutParameters = new(ViewGroup.LayoutParams.WrapContent,
                                                        ViewGroup.LayoutParams.WrapContent,
                                                        GravityFlags.Center);
        container.AddView(bannerView, layoutParameters);
        container.RequestLayout();
        this.VirtualView.InvalidateMeasure();
    }

    private void OnContainerWidthChanged(int width)
    {
        if (this.VirtualView.AdSize != BannerAdSize.Adaptive ||
            this.PlatformView is not BannerContainer container)
            return;

        int availableWidth = this.GetAvailableWidth(container);
        if (availableWidth <= 0 || availableWidth == this.lastAdaptiveWidth)
            return;

        this.LoadBanner(container, availableWidth);
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

    private class BannerContainer(Context context, Action<int> widthChanged)
        : FrameLayout(context)
    {
        #region Properties

        internal Action<int>? WidthChanged { get; set; } = widthChanged;

        #endregion

        #region Methods

        protected override void OnSizeChanged(int width, int height, int oldWidth, int oldHeight)
        {
            base.OnSizeChanged(width, height, oldWidth, oldHeight);

            if (width > 0 && width != oldWidth)
                this.WidthChanged?.Invoke(width);
        }

        #endregion
    }

    #endregion
}

#endif
