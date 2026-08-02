using AMDevIT.Admob.Wrapper.MAUICross.Services;
using Microsoft.Extensions.Logging;

namespace AMDevIT.Admob.Wrapper.MAUICross.Tests;

[TestClass]
public sealed class AdMobConsentServiceUnsupportedTests
{
    #region Methods

    [TestMethod]
    public void Properties_DescribeSafeUnsupportedState()
    {
        TestLogger<AdMobConsentService> logger = new();
        AdMobConsentService service = new(logger);

        Assert.IsFalse(service.IsSupported);
        Assert.IsTrue(service.CanRequestAds);
        Assert.IsTrue(service.IsInitialized);
        Assert.IsNotNull(service.CurrentConsentInformation);
        Assert.AreEqual(ConsentStatus.NotRequired, service.CurrentConsentInformation.ConsentStatus);
        Assert.AreEqual(
            PrivacyOptionsRequirementStatus.NotRequired,
            service.CurrentConsentInformation.PrivacyOptionsRequirementStatus);
        Assert.IsTrue(logger.Entries.Any(entry =>
            entry.Level == LogLevel.Warning &&
            entry.Message.Contains("not supported", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public async Task Operations_ReturnNeutralResultsWithoutThrowing()
    {
        TestLogger<AdMobConsentService> logger = new();
        AdMobConsentService service = new(logger);
        using CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.Cancel();

        await service.InitializeAsync("unused", cancellationTokenSource.Token);
        ConsentInformationSnapshot snapshot = await service.UpdateCurrentConsentInformationAsync(
            cancellationToken: cancellationTokenSource.Token);
        ConsentGatheringResult gatheringResult = await service.GatherConsentAsync(
            cancellationToken: cancellationTokenSource.Token);
        await service.ShowPrivacyOptionsFormAsync(cancellationTokenSource.Token);
        await service.LoadAndShowConsentFormIfRequiredAsync(cancellationTokenSource.Token);
        service.ResetConsentForTesting();

        Assert.AreEqual(ConsentStatus.NotRequired, snapshot.ConsentStatus);
        Assert.AreEqual(
            PrivacyOptionsRequirementStatus.NotRequired,
            snapshot.PrivacyOptionsRequirementStatus);
        Assert.IsTrue(gatheringResult.CanRequestAds);
        Assert.IsFalse(gatheringResult.PrivacyOptionsRequired);
        Assert.IsTrue(logger.Entries.Count(entry => entry.Level == LogLevel.Debug) >= 6);
    }

    #endregion

    private sealed class TestLogger<T> : ILogger<T>
    {
        #region Fields

        private readonly List<(LogLevel Level, string Message)> entries = [];

        #endregion

        #region Properties

        public IReadOnlyList<(LogLevel Level, string Message)> Entries => this.entries;

        #endregion

        #region Methods

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
        {
            _ = state;
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            _ = logLevel;
            return true;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            _ = eventId;
            this.entries.Add((logLevel, formatter(state, exception)));
        }

        #endregion
    }
}
