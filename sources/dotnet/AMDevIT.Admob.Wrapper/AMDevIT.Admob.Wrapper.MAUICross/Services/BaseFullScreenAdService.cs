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

    private readonly object loadSyncRoot = new();
    private volatile bool disposedValue;
    private TaskCompletionSource? loadCompletionSource;
    private CancellationTokenRegistration loadCancellationRegistration;

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

    public async Task LoadAndShowAsync(string adUnitId, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(this.Disposed, this);
        await this.LoadAsync(adUnitId, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();

        if (!this.IsLoaded)
            throw new InvalidOperationException($"Cannot show {this.AdTypeName} because it is not loaded.");

        this.Show();
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected abstract void DisposeObjects();

    protected Task StartLoadAsync(string adUnitId,
                                  Action startLoad,
                                  CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(adUnitId);
        ArgumentNullException.ThrowIfNull(startLoad);
        cancellationToken.ThrowIfCancellationRequested();

        TaskCompletionSource taskCompletionSource;

        lock (this.loadSyncRoot)
        {
            ObjectDisposedException.ThrowIf(this.disposedValue, this);

            if (this.loadCompletionSource != null)
                throw new InvalidOperationException($"A {this.AdTypeName} load operation is already in progress.");

            if (this.IsShowing)
                throw new InvalidOperationException($"Cannot load {this.AdTypeName} while it is being shown.");

            this.IsLoaded = false;
            taskCompletionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);
            this.loadCompletionSource = taskCompletionSource;
            this.loadCancellationRegistration = cancellationToken.Register(() =>
                this.CancelLoadAwait(cancellationToken));
        }

        try
        {
            startLoad();
        }
        catch (Exception exc)
        {
            this.CompleteLoadFailure(exc);
        }

        return taskCompletionSource.Task;
    }

    protected void CompleteLoadSuccess()
    {
        TaskCompletionSource? taskCompletionSource;
        CancellationTokenRegistration cancellationRegistration;

        lock (this.loadSyncRoot)
        {
            if (this.disposedValue || this.loadCompletionSource == null)
                return;

            this.IsLoaded = true;
            this.IsShowing = false;
            taskCompletionSource = this.loadCompletionSource;
            this.loadCompletionSource = null;
            cancellationRegistration = this.loadCancellationRegistration;
            this.loadCancellationRegistration = default;
        }

        cancellationRegistration.Dispose();
        taskCompletionSource.TrySetResult();
    }

    protected void CompleteLoadFailure(long errorCode, string errorMessage)
    {
        this.CompleteLoadFailure(new AdLoadException(errorCode, errorMessage));
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
        TaskCompletionSource? taskCompletionSource;
        CancellationTokenRegistration cancellationRegistration;

        lock (this.loadSyncRoot)
        {
            if (this.disposedValue)
                return;

            this.disposedValue = true;
            this.IsLoaded = false;
            this.IsShowing = false;
            taskCompletionSource = this.loadCompletionSource;
            this.loadCompletionSource = null;
            cancellationRegistration = this.loadCancellationRegistration;
            this.loadCancellationRegistration = default;
        }

        cancellationRegistration.Dispose();
        taskCompletionSource?.TrySetException(new ObjectDisposedException(this.GetType().FullName));

        if (disposing)
            this.DisposeObjects();
    }

    private void CancelLoadAwait(CancellationToken cancellationToken)
    {
        lock (this.loadSyncRoot)
            this.loadCompletionSource?.TrySetCanceled(cancellationToken);
    }

    private void CompleteLoadFailure(Exception exception)
    {
        TaskCompletionSource? taskCompletionSource;
        CancellationTokenRegistration cancellationRegistration;

        lock (this.loadSyncRoot)
        {
            if (this.disposedValue || this.loadCompletionSource == null)
                return;

            this.IsLoaded = false;
            this.IsShowing = false;
            taskCompletionSource = this.loadCompletionSource;
            this.loadCompletionSource = null;
            cancellationRegistration = this.loadCancellationRegistration;
            this.loadCancellationRegistration = default;
        }

        cancellationRegistration.Dispose();
        taskCompletionSource.TrySetException(exception);
    }

    #endregion
}
