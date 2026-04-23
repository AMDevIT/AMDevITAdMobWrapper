using Microsoft.Extensions.Logging;

namespace AMDevIT.Admob.Wrapper.MAUICross.Services;

public partial class InterstitialAdService
    : IInterstitialAdService, IDisposable
{    
    #region Events

    public event EventHandler? AdLoaded;
    public event EventHandler<AdFailedEventArgs>? AdFailedToLoad;
    public event EventHandler? AdShown;
    public event EventHandler? AdDismissed;
    public event EventHandler? AdClicked;
    public event EventHandler? AdImpression;
    public event EventHandler<AdFailedEventArgs>? AdFailedToShow;

    #endregion

    #region Fields

    private bool disposedValue;

    #endregion

    #region Properties

    protected ILogger<InterstitialAdService> Logger { get; }

    public bool Disposed => this.disposedValue;

    #endregion

    #region Methods

    protected void OnAdLoaded() => MainThread.BeginInvokeOnMainThread(() => this.AdLoaded?.Invoke(this, EventArgs.Empty));

    protected void OnAdFailedToLoad(int code, string msg)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            AdFailedEventArgs args = new(code, msg);
            this.AdFailedToLoad?.Invoke(this, args);
        });
    }

    protected void OnAdShown() => MainThread.BeginInvokeOnMainThread(() => this.AdShown?.Invoke(this, EventArgs.Empty));
    protected void OnAdDismissed() => MainThread.BeginInvokeOnMainThread(() => this.AdDismissed?.Invoke(this, EventArgs.Empty));
    protected void OnAdClicked() => MainThread.BeginInvokeOnMainThread(() => this.AdClicked?.Invoke(this, EventArgs.Empty));
    protected void OnAdImpression() => MainThread.BeginInvokeOnMainThread(() => this.AdImpression?.Invoke(this, EventArgs.Empty));
    protected void OnAdFailedToShow(int code, string msg)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            AdFailedEventArgs args = new(code, msg);
            this.AdFailedToShow?.Invoke(this, args);
        });
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                this.DisposeObjects();
            }

            this.disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
