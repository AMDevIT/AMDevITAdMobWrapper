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
    private readonly OnAdLoadedListener onAdLoadedListener;
    private readonly OnAdEventListener onAdEventListener;

    #endregion

    #region Properties    

    public bool IsShowing => throw new NotImplementedException();

    public bool IsLoaded => throw new NotImplementedException();

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

    public Task LoadAsync(string adUnitId)
    {
        ObjectDisposedException.ThrowIf(this.Disposed, this);

        Context context = Android.App.Application.Context;        
        this.wrapper ??= new InterstitialAdWrapper(context);

        TaskCompletionSource taskCompletionSource = new ();

        this.wrapper.Load(adUnitId,
                          this.onAdLoadedListener,
                          this.onAdEventListener);
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
    }

    #endregion

    #region Event Handlers

    #region Ad Loaded Listener

    private void OnAdLoadedListener_AdFailedToLoad(object? sender, AdFailedToLoadEventArgs e)
    {
        this.OnAdFailedToLoad(e.ErrorCode, e.ErrorMessage);
    }

    private void OnAdLoadedListener_AdLoaded(object? sender, EventArgs e)
    {
        this.OnAdLoaded();
    }

    #endregion

    #region Ad Event Listener

    private void OnAdEventListener_AdFailedToShow(object? sender, AdFailedToShowEventArgs e)
    {
        this.OnAdFailedToShow(e.ErrorCode, e.ErrorMessage);
    }

    private void OnAdEventListener_AdDismissed(object? sender, EventArgs e)
    {
        this.OnAdDismissed();
    }

    private void OnAdEventListener_AdImpression(object? sender, EventArgs e)
    {
        this.OnAdImpression();
    }

    private void OnAdEventListener_AdShown(object? sender, EventArgs e)
    {
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