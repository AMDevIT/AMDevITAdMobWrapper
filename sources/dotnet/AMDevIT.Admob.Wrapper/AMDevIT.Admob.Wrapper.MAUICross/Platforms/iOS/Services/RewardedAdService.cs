#if IOS

using AMDevIT.Admob.Wrapper.iOSNative;
using AMDevIT.Admob.Wrapper.MAUICross.Platforms.iOS.Listeners;
using Microsoft.Extensions.Logging;
using UIKit;

namespace AMDevIT.Admob.Wrapper.MAUICross.Services;

public partial class RewardedAdService
{
    #region Fields

    private RewardedAdWrapper? wrapper;
    private readonly AppleRewardListener onAdRewardListener;
    private readonly AppleOnAdEventListener onAdEventListener;
    private readonly AppleOnAdLoadedListener onAdLoadedListener;


    #endregion

    #region .ctor

    public RewardedAdService(ILogger<AppOpenAdService> logger,
                             IContextResolverService contextResolverService)
        : base(logger, contextResolverService)
    {
        this.onAdRewardListener = new();
        this.onAdRewardListener.RewardEarned += OnAdRewardListener_RewardEarned;

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

    public override Task LoadAsync(string adUnitId, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(this.Disposed, this);

        TaskCompletionSource taskCompletionSource = new();

        this.wrapper ??= new RewardedAdWrapper();

        cancellationToken.Register(() => taskCompletionSource.TrySetCanceled(cancellationToken));
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            this.wrapper.LoadWithAdUnitId(adUnitId, this.onAdLoadedListener, this.onAdEventListener);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exc)
        {
            this.Logger.LogError(exc, "Failed to load rewarded ad with ad unit id {AdUnitId}", adUnitId);
            taskCompletionSource.SetException(exc);
        }

        return taskCompletionSource.Task;
    }

    public override void Show()
    {
        ObjectDisposedException.ThrowIf(this.Disposed, this);

        if (this.wrapper == null)
        {
            this.Logger.LogError("Rewarded ad wrapper is not initialized. Call LoadAsync first.");
            throw new InvalidOperationException("Rewarded ad wrapper is not initialized. Call LoadAsync first.");
        }

        if (!this.IsLoaded)
        {
            this.Logger.LogWarning("Cannot show rewarded ad because it is not loaded.");
            throw new InvalidOperationException("Cannot show rewarded ad because it is not loaded.");
        }

        UIViewController? viewController = this.ContextResolverService.GetViewController();

        if (viewController == null)
        {
            this.Logger.LogError("Failed to get top view controller to show rewarded ad.");
            throw new InvalidOperationException("Failed to get top view controller to show rewarded ad.");
        }

        try
        {
            this.wrapper.ShowWithViewController(viewController, this.onAdRewardListener);
        }
        catch (Exception exc)
        {
            this.Logger.LogError(exc, "Failed to show rewarded ad.");
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

    #region Ad Reward Listener

    private void OnAdRewardListener_RewardEarned(object? sender, AdReward e)
    {
        this.OnAdRewardEarned(e);
    }

    #endregion

    #region Ad Loaded Listener

    private void OnAdLoadedListener_AdFailedToLoad(object? sender, AdFailedToLoadArgs e)
    {
        this.IsLoaded = false;
        this.IsShowing = false;

        this.OnAdFailedToLoad(e.ErrorCode, e.ErrorMessage);
    }

    private void OnAdLoadedListener_AdLoaded(object? sender, EventArgs e)
    {
        this.IsLoaded = true;
        this.IsShowing = false;

        this.OnAdLoaded();
    }

    #endregion

    #region Ad Event Listener

    private void OnAdEventListener_AdFailedToShow(object? sender, AdFailedToShowArgs e)
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

    #endregion
}

#endif