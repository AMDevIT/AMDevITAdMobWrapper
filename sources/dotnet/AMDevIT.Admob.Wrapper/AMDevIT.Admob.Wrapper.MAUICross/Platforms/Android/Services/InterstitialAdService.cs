#if ANDROID

using AMDevIT.Admob.Wrapper.Ads;
using AMDevIT.Admob.Wrapper.Listeners;
using AMDevIT.Admob.Wrapper.MAUICross.Platforms.Android.Listeners;
using Android.App;
using Android.Content;
using Microsoft.Extensions.Logging;

namespace AMDevIT.Admob.Wrapper.MAUICross.Services;

public partial class InterstitialAdService
{
    #region Fields

    private InterstitialAdWrapper? wrapper;
    private readonly DroidOnAdLoadedListener onAdLoadedListener;
    private readonly DroidOnAdEventListener onAdEventListener;

    #endregion
    
    #region .ctor

    public InterstitialAdService(ILogger<InterstitialAdService> logger)
    {
        this.Logger = logger;

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

    public Task LoadAsync(string adUnitId, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(this.Disposed, this);

        TaskCompletionSource taskCompletionSource = new();
        Context context = Android.App.Application.Context;        

        this.wrapper ??= new InterstitialAdWrapper(context);      

        cancellationToken.Register(() => taskCompletionSource.TrySetCanceled(cancellationToken));
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            this.wrapper.Load(adUnitId,
                              this.onAdLoadedListener,
                              this.onAdEventListener);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch(Exception exc)
        {
            taskCompletionSource.SetException(exc);
        }
        return taskCompletionSource.Task;
    }

    public void Show()
    {
        ObjectDisposedException.ThrowIf(this.Disposed, this);
        Activity? activity;

        if (this.wrapper == null)
        {
            if (this.Logger.IsEnabled(LogLevel.Error))
                this.Logger.LogError("Interstitial ad wrapper is not initialized. Call LoadAsync first.");
            throw new InvalidOperationException("Interstitial ad wrapper is not initialized. Call LoadAsync first.");
        }

        if (!this.IsLoaded)
        {
            if (this.Logger.IsEnabled(LogLevel.Warning))
                this.Logger.LogWarning("Cannot show interstitial ad because it is not loaded.");
            throw new InvalidOperationException("Cannot show interstitial ad because it is not loaded.");
        }               

        try
        {
            activity = Platform.CurrentActivity;
        }
        catch(Exception exc)
        {
            if (this.Logger.IsEnabled(LogLevel.Error))
                this.Logger.LogError(exc, "Failed to retrieve current activity.");
            activity = null;
        }

        if (activity == null) 
            return;

        this.wrapper.Show(activity, this.onAdLoadedListener);
    }    

    protected virtual void DisposeObjects()
    {
        this.onAdLoadedListener.AdLoaded -= OnAdLoadedListener_AdLoaded;
        this.onAdLoadedListener.AdFailedToLoad -= OnAdLoadedListener_AdFailedToLoad;

        this.onAdEventListener.AdClicked -= OnAdEventListener_AdClicked;
        this.onAdEventListener.AdShown -= OnAdEventListener_AdShown;
        this.onAdEventListener.AdImpression -= OnAdEventListener_AdImpression;
        this.onAdEventListener.AdDismissed -= OnAdEventListener_AdDismissed;
        this.onAdEventListener.AdFailedToShow -= OnAdEventListener_AdFailedToShow;
    }

    #endregion

    #region Event Handlers

    #region Ad Loaded Listener

    private void OnAdLoadedListener_AdFailedToLoad(object? sender, AdFailedToLoadEventArgs e)
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

    #endregion
}

#endif