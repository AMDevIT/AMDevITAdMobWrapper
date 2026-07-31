#if IOS

using AMDevIT.Admob.Wrapper.iOSNative;
using AMDevIT.Admob.Wrapper.MAUICross.Platforms.iOS.Listeners;
using Microsoft.Extensions.Logging;
using UIKit;

namespace AMDevIT.Admob.Wrapper.MAUICross.Services;

public partial class AppOpenAdService
    : BaseFullScreenAdService, IAppOpenAdService
{
    #region Fields

    private AppOpenAdWrapper? wrapper;
    private AppleOnAdEventListener onAdEventListener;
    private AppleOnAdLoadedListener onAdLoadedListener;

    #endregion

    #region Properties

    #endregion

    #region .ctor

    public AppOpenAdService(ILogger<AppOpenAdService> logger,
                            IContextResolverService contextResolverService)
        : base(logger, contextResolverService)
    {
        this.onAdLoadedListener = new();
        this.onAdLoadedListener.AdLoaded += OnAdLoadedListener_AdLoaded;
        this.onAdLoadedListener.AdFailedToLoad += OnAdLoadedListener_AdFailedToLoad;

        this.onAdEventListener = new();
        this.onAdEventListener.AdClicked += OnAdEventListener_AdClicked;
        this.onAdEventListener.AdShown += OnAdEventListener_AdShown;
        this.onAdEventListener.AdDismissed += OnAdEventListener_AdDismissed;
        this.onAdEventListener.AdImpression += OnAdEventListener_AdImpression;
        this.onAdEventListener.AdFailedToShow += OnAdEventListener_AdFailedToShow;
    }

    #endregion

    #region Methods

    public override Task LoadAsync(string adUnitId, CancellationToken cancellationToken = default)
    {
        return this.StartLoadAsync(adUnitId, () =>
        {
            this.wrapper ??= new AppOpenAdWrapper();
            this.wrapper.LoadWithAdUnitId(adUnitId,
                                          this.onAdLoadedListener,
                                          this.onAdEventListener);
        }, cancellationToken);
    }

    public override void Show()
    {
        ObjectDisposedException.ThrowIf(this.Disposed, this);
        UIViewController? viewController;

        if (this.wrapper == null)
        {
            if (this.Logger.IsEnabled(LogLevel.Error))
                this.Logger.LogError("App Open ad wrapper is not initialized. Call LoadAsync first.");
            throw new InvalidOperationException("App Open ad wrapper is not initialized. Call LoadAsync first.");
        }

        if (!this.IsLoaded)
        {
            if (this.Logger.IsEnabled(LogLevel.Warning))
                this.Logger.LogWarning("Cannot show App Open ad because it is not loaded.");
            throw new InvalidOperationException("Cannot show App Open ad because it is not loaded.");
        }

        // viewController = ViewControllerHelper.GetTopViewController();
        viewController = this.ContextResolverService.GetViewController();
        if (viewController == null)
        {
            if (this.Logger.IsEnabled(LogLevel.Error))
                this.Logger.LogError("Failed to get top view controller to show App Open ad.");
            throw new InvalidOperationException("Failed to get top view controller to show App Open ad.");
        }

        try
        {
            this.wrapper.ShowWithViewController(viewController);
        }
        catch (Exception exc)
        {
            if (this.Logger.IsEnabled(LogLevel.Error))
                this.Logger.LogError(exc, "Failed to show App Open ad.");
            throw;
        }
    }

    protected override void DisposeObjects()
    {
        this.onAdLoadedListener.AdLoaded -= OnAdLoadedListener_AdLoaded;
        this.onAdLoadedListener.AdFailedToLoad -= OnAdLoadedListener_AdFailedToLoad;

        this.onAdEventListener.AdClicked -= OnAdEventListener_AdClicked;
        this.onAdEventListener.AdShown -= OnAdEventListener_AdShown;
        this.onAdEventListener.AdDismissed -= OnAdEventListener_AdDismissed;
        this.onAdEventListener.AdImpression -= OnAdEventListener_AdImpression;
        this.onAdEventListener.AdFailedToShow -= OnAdEventListener_AdFailedToShow;

        try
        {
            this.wrapper?.Dispose();
        }
        catch (Exception)
        {

        }
    }

    #endregion

    #region Event Handlers

    private void OnAdLoadedListener_AdFailedToLoad(object? sender, AdFailedToLoadArgs e)
    {
        this.CompleteLoadFailure(e.ErrorCode, e.ErrorMessage);
        this.OnAdFailedToLoad(e.ErrorCode, e.ErrorMessage);
    }

    private void OnAdLoadedListener_AdLoaded(object? sender, EventArgs e)
    {
        this.CompleteLoadSuccess();
        this.OnAdLoaded();
    }

    private void OnAdEventListener_AdFailedToShow(object? sender, AdFailedToShowArgs e)
    {
        this.IsShowing = false;
        this.OnAdFailedToShow(e.ErrorCode, e.ErrorMessage);
    }

    private void OnAdEventListener_AdImpression(object? sender, EventArgs e)
    {
        this.OnAdImpression();
    }

    private void OnAdEventListener_AdDismissed(object? sender, EventArgs e)
    {
        this.IsLoaded = false;
        this.IsShowing = false;
        this.OnAdDismissed();
    }

    private void OnAdEventListener_AdShown(object? sender, EventArgs e)
    {
        this.IsShowing = true;
        this.OnAdShown();
    }

    private void OnAdEventListener_AdClicked(object? sender, EventArgs e)
    {
        this.OnAdClicked();
    }

    #endregion
}

#endif
