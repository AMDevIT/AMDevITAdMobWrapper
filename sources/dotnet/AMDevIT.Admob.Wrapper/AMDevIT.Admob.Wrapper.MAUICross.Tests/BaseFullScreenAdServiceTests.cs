using AMDevIT.Admob.Wrapper.MAUICross.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace AMDevIT.Admob.Wrapper.MAUICross.Tests;

[TestClass]
public sealed class BaseFullScreenAdServiceTests
{
    #region Methods

    [TestMethod]
    public async Task LoadAsync_CompletesAfterNativeSuccess()
    {
        using TestFullScreenAdService service = new();

        Task loadTask = service.LoadAsync("test-ad-unit");

        Assert.IsFalse(loadTask.IsCompleted);
        Assert.AreEqual(1, service.StartCount);

        service.CompleteNativeLoad();
        await loadTask;

        Assert.IsTrue(service.IsLoaded);
    }

    [TestMethod]
    public async Task LoadAsync_FaultsWithAdLoadExceptionAfterNativeFailure()
    {
        using TestFullScreenAdService service = new();

        Task loadTask = service.LoadAsync("test-ad-unit");
        service.FailNativeLoad(42, "No fill");

        AdLoadException exception = await AssertThrowsAsync<AdLoadException>(() => loadTask);

        Assert.AreEqual(42, exception.ErrorCode);
        Assert.AreEqual("No fill", exception.Message);
        Assert.IsFalse(service.IsLoaded);
    }

    [TestMethod]
    public async Task LoadAsync_CancellationCancelsAwaitButKeepsNativeOperationIsolated()
    {
        using TestFullScreenAdService service = new();
        using CancellationTokenSource cancellationTokenSource = new();

        Task canceledLoadTask = service.LoadAsync("first-ad-unit", cancellationTokenSource.Token);
        cancellationTokenSource.Cancel();

        await AssertThrowsAsync<OperationCanceledException>(() => canceledLoadTask);
        AssertThrows<InvalidOperationException>(() => service.LoadAsync("second-ad-unit"));

        service.CompleteNativeLoad();
        Assert.IsTrue(service.IsLoaded);

        Task nextLoadTask = service.LoadAsync("second-ad-unit");
        service.CompleteNativeLoad();
        await nextLoadTask;

        Assert.AreEqual(2, service.StartCount);
    }

    [TestMethod]
    public async Task LoadAsync_RejectsConcurrentLoads()
    {
        using TestFullScreenAdService service = new();

        Task firstLoadTask = service.LoadAsync("first-ad-unit");

        AssertThrows<InvalidOperationException>(() => service.LoadAsync("second-ad-unit"));

        service.CompleteNativeLoad();
        await firstLoadTask;
    }

    [TestMethod]
    public async Task LoadAsync_SynchronousStartFailureFaultsTaskAndAllowsRetry()
    {
        using TestFullScreenAdService service = new()
        {
            StartException = new InvalidOperationException("Native start failed")
        };

        Task failedLoadTask = service.LoadAsync("first-ad-unit");
        InvalidOperationException exception = await AssertThrowsAsync<InvalidOperationException>(() => failedLoadTask);

        Assert.AreEqual("Native start failed", exception.Message);

        service.StartException = null;
        Task nextLoadTask = service.LoadAsync("second-ad-unit");
        service.CompleteNativeLoad();
        await nextLoadTask;
    }

    [TestMethod]
    public async Task Dispose_FaultsPendingLoadAndRejectsFutureLoads()
    {
        TestFullScreenAdService service = new();
        Task loadTask = service.LoadAsync("test-ad-unit");

        service.Dispose();

        await AssertThrowsAsync<ObjectDisposedException>(() => loadTask);
        Assert.IsTrue(service.DisposeObjectsCalled);
        AssertThrows<ObjectDisposedException>(() => service.LoadAsync("next-ad-unit"));
    }

    [TestMethod]
    public async Task LoadAndShowAsync_ShowsOnlyAfterLoadCompletes()
    {
        using TestFullScreenAdService service = new();

        Task loadAndShowTask = service.LoadAndShowAsync("test-ad-unit");

        Assert.AreEqual(0, service.ShowCount);
        service.CompleteNativeLoad();
        await loadAndShowTask;

        Assert.AreEqual(1, service.ShowCount);
    }

    private static TException AssertThrows<TException>(Action action)
        where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException exception)
        {
            return exception;
        }
        catch (Exception exception)
        {
            Assert.Fail($"Expected {typeof(TException).Name}, but caught {exception.GetType().Name}.");
        }

        Assert.Fail($"Expected {typeof(TException).Name}, but no exception was thrown.");
        throw new InvalidOperationException();
    }

    private static async Task<TException> AssertThrowsAsync<TException>(Func<Task> action)
        where TException : Exception
    {
        try
        {
            await action();
        }
        catch (TException exception)
        {
            return exception;
        }
        catch (Exception exception)
        {
            Assert.Fail($"Expected {typeof(TException).Name}, but caught {exception.GetType().Name}.");
        }

        Assert.Fail($"Expected {typeof(TException).Name}, but no exception was thrown.");
        throw new InvalidOperationException();
    }

    #endregion

    private sealed class TestFullScreenAdService()
        : BaseFullScreenAdService(NullLogger.Instance, new TestContextResolverService())
    {
        #region Properties

        public int ShowCount { get; private set; }
        public int StartCount { get; private set; }
        public bool DisposeObjectsCalled { get; private set; }
        public Exception? StartException { get; set; }

        protected override string AdTypeName => "test ad";

        #endregion

        #region Methods

        public override Task LoadAsync(string adUnitId, CancellationToken cancellationToken = default)
        {
            return this.StartLoadAsync(adUnitId, () =>
            {
                this.StartCount++;

                if (this.StartException != null)
                    throw this.StartException;
            }, cancellationToken);
        }

        public override void Show()
        {
            this.ShowCount++;
        }

        public void CompleteNativeLoad()
        {
            this.CompleteLoadSuccess();
        }

        public void FailNativeLoad(long errorCode, string errorMessage)
        {
            this.CompleteLoadFailure(errorCode, errorMessage);
        }

        protected override void DisposeObjects()
        {
            this.DisposeObjectsCalled = true;
        }

        #endregion
    }

    private sealed class TestContextResolverService : IContextResolverService
    {
        #region Methods

        public object? GetPlatformContext()
        {
            return null;
        }

        #endregion
    }
}
