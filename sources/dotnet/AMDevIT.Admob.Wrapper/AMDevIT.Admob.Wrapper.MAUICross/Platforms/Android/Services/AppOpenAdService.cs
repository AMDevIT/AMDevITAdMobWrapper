#if ANDROID

using AMDevIT.Admob.Wrapper.Ads;
using AMDevIT.Admob.Wrapper.Listeners;
using AMDevIT.Admob.Wrapper.MAUICross.Platforms.Android.Diagnostics;
using AMDevIT.Admob.Wrapper.MAUICross.Platforms.Android.Listeners;
using Android.App;
using Microsoft.Extensions.Logging;

namespace AMDevIT.Admob.Wrapper.MAUICross.Services;

public partial class AppOpenAdService
    : BaseFullScreenAdService, IAppOpenAdService
{
    #region Fields

    private AppOpenAdWrapper? wrapper;
    private readonly DroidLoggerAdapter loggerAdapter;
    private readonly DroidOnAdLoadedListener onAdLoadedListener;
    private readonly DroidOnAdEventListener onAdEventListener;

    #endregion

    #region .ctor  

    public AppOpenAdService(ILogger<AppOpenAdService> logger, 
                            IContextResolverService contextResolverService)
        : base(logger, contextResolverService)
    {
        this.loggerAdapter = new DroidLoggerAdapter(logger);
        this.onAdLoadedListener = new();

        this.onAdLoadedListener.AdLoaded += OnAdLoadedListener_AdLoaded;
        this.onAdLoadedListener.AdFailedToLoad += OnAdLoadedListener_AdFailedToLoad;

        this.onAdEventListener = new();
        
        this.onAdEventListener.AdClicked += OnAdEventListener_AdClicked;
        this.onAdEventListener.AdShown += OnAdEventListener_AdShown;
        this.onAdEventListener.AdImpression += OnAdEventListener_AdImpression;
        this.onAdEventListener.AdDismissed += OnAdEventListener_AdDismissed;
        this.onAdEventListener.AdFailedToShow += OnAdEventListener_AdFailedToShow;
    }

    #endregion

    #region Methods

    public override Task LoadAsync(string adUnitId, CancellationToken cancellationToken)
    {
        return this.StartLoadAsync(adUnitId, () =>
        {
            this.wrapper ??= new AppOpenAdWrapper(this.loggerAdapter);
            this.wrapper.Load(adUnitId,
                              this.onAdLoadedListener,
                              this.onAdEventListener);
        }, cancellationToken);
    }   

    public override void Show()
    {
        ObjectDisposedException.ThrowIf(this.Disposed, this);
        Activity? activity;

        if (this.wrapper == null)
        {
            if (this.Logger.IsEnabled(LogLevel.Error))
                this.Logger.LogError("App Open ad wrapper is not initialized. Call LoadAsync first.");
            throw new InvalidOperationException("App Open ad wrapper is not initialized. Call LoadAsync first.");
        }

        if (!this.IsLoaded)
        {
            if (this.Logger.IsEnabled(LogLevel.Warning))
                this.Logger.LogWarning("Cannot show app open ad because it is not loaded.");
            throw new InvalidOperationException("Cannot show app open ad because it is not loaded.");
        }

        try
        {
            activity = this.ContextResolverService.GetContext() as Activity ?? throw new InvalidOperationException("Context cannot be null"); 
        }
        catch (Exception exc)
        {
            if (this.Logger.IsEnabled(LogLevel.Error))
                this.Logger.LogError(exc, "Failed to retrieve current activity.");
            activity = null;
        }

        if (activity == null)
            return;

        this.wrapper.Show(activity, this.onAdLoadedListener);
    }

    protected override void DisposeObjects()
    {
        this.onAdLoadedListener.AdLoaded -= OnAdLoadedListener_AdLoaded;
        this.onAdLoadedListener.AdFailedToLoad -= OnAdLoadedListener_AdFailedToLoad;

        this.onAdEventListener.AdClicked -= OnAdEventListener_AdClicked;
        this.onAdEventListener.AdShown -= OnAdEventListener_AdShown;
        this.onAdEventListener.AdImpression -= OnAdEventListener_AdImpression;
        this.onAdEventListener.AdDismissed -= OnAdEventListener_AdDismissed;
        this.onAdEventListener.AdFailedToShow -= OnAdEventListener_AdFailedToShow;
        this.wrapper?.Dispose();
        this.wrapper = null;
        this.loggerAdapter.Dispose();
    }

    #endregion

    #region Event handlers

    private void OnAdLoadedListener_AdFailedToLoad(object? sender, AdFailedToLoadEventArgs e)
    {
        this.CompleteLoadFailure(e.ErrorCode, e.ErrorMessage);
        this.OnAdFailedToLoad(e.ErrorCode, e.ErrorMessage);
    }

    private void OnAdLoadedListener_AdLoaded(object? sender, EventArgs e)
    {
        this.CompleteLoadSuccess();
        this.OnAdLoaded();
    }

    private void OnAdEventListener_AdFailedToShow(object? sender, AdFailedToShowEventArgs e)
    {
        this.IsShowing = false;

        this.OnAdFailedToShow(e.ErrorCode, e.ErrorMessage);
    }

    private void OnAdEventListener_AdDismissed(object? sender, EventArgs e)
    {
        this.IsLoaded = false;
        this.IsShowing = false;

        this.OnAdDismissed();
    }

    private void OnAdEventListener_AdImpression(object? sender, EventArgs e)
    {
        this.OnAdImpression();
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
