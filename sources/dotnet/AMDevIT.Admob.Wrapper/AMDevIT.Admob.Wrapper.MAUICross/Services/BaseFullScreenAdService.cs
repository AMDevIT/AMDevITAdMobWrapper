using Microsoft.Extensions.Logging;

namespace AMDevIT.Admob.Wrapper.MAUICross.Services;

public abstract class BaseFullScreenAdService : IDisposable
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

    protected ILogger Logger { get; }
    protected IContextResolverService ContextResolverService { get; }

    public bool Disposed => this.disposedValue;

    public bool IsLoaded { get; protected set; }
    public bool IsShowing { get; protected set; }

    protected abstract string AdTypeName { get; }

    #endregion

    #region .ctor

    public BaseFullScreenAdService(ILogger logger,
                                   IContextResolverService contextResolverService)
    {
        this.Logger = logger;
        this.ContextResolverService = contextResolverService;
    }

    #endregion

    #region Methods

    public abstract Task LoadAsync(string adUnitId, CancellationToken cancellationToken = default);
    public abstract void Show();
    protected abstract void DisposeObjects();

    public async Task LoadAndShowAsync(string adUnitId, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(this.Disposed, this);
        await this.LoadAsync(adUnitId, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();

        if (!this.IsLoaded)
            throw new InvalidOperationException($"Cannot show {this.AdTypeName} because it is not loaded.");

        this.Show();
    }

    protected void OnAdLoaded() => MainThread.BeginInvokeOnMainThread(() => this.AdLoaded?.Invoke(this, EventArgs.Empty));

    protected void OnAdShown() => MainThread.BeginInvokeOnMainThread(() => this.AdShown?.Invoke(this, EventArgs.Empty));

    protected void OnAdDismissed() => MainThread.BeginInvokeOnMainThread(() => this.AdDismissed?.Invoke(this, EventArgs.Empty));

    protected void OnAdClicked() => MainThread.BeginInvokeOnMainThread(() => this.AdClicked?.Invoke(this, EventArgs.Empty));

    protected void OnAdImpression() => MainThread.BeginInvokeOnMainThread(() => this.AdImpression?.Invoke(this, EventArgs.Empty));

    protected void OnAdFailedToLoad(long code, string msg) => MainThread.BeginInvokeOnMainThread(() => this.AdFailedToLoad?.Invoke(this, new AdFailedEventArgs(code, msg)));

    protected void OnAdFailedToShow(long code, string msg) => MainThread.BeginInvokeOnMainThread(() => this.AdFailedToShow?.Invoke(this, new AdFailedEventArgs(code, msg)));

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
                this.DisposeObjects();

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