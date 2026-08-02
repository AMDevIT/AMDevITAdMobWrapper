#if WINDOWS || MACCATALYST

using Microsoft.Extensions.Logging;

namespace AMDevIT.Admob.Wrapper.MAUICross.Services;

public sealed class AdMobConsentService : IAdMobConsentService
{
    #region Const

#if WINDOWS
    private const string PlatformName = "Windows";
#else
    private const string PlatformName = "Mac Catalyst";
#endif

    #endregion

    #region Fields

    private static readonly ConsentInformationSnapshot UnsupportedConsentInformation =
        new(null, ConsentStatus.NotRequired, PrivacyOptionsRequirementStatus.NotRequired);
    private static readonly ConsentGatheringResult UnsupportedGatheringResult =
        new(true, false);
    private readonly ILogger<AdMobConsentService> logger;

    #endregion

    #region Properties

    public bool IsSupported => false;

    public bool CanRequestAds => true;

    public bool IsInitialized => true;

    public ConsentInformationSnapshot CurrentConsentInformation => UnsupportedConsentInformation;

    #endregion

    #region .ctor

    public AdMobConsentService(ILogger<AdMobConsentService> logger)
    {
        this.logger = logger;
        this.logger.LogWarning(
            "AdMob consent is not supported on {PlatformName}. A no-op service will be used.",
            PlatformName);
    }

    #endregion

    #region Methods

    public Task InitializeAsync(string applicationId, CancellationToken cancellationToken = default)
    {
        _ = applicationId;
        _ = cancellationToken;
        this.LogSkippedOperation(nameof(InitializeAsync));
        return Task.CompletedTask;
    }

    public Task<ConsentInformationSnapshot> UpdateCurrentConsentInformationAsync(
        ConsentRequestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        _ = options;
        _ = cancellationToken;
        this.LogSkippedOperation(nameof(UpdateCurrentConsentInformationAsync));
        return Task.FromResult(UnsupportedConsentInformation);
    }

    public Task<ConsentGatheringResult> GatherConsentAsync(
        ConsentRequestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        _ = options;
        _ = cancellationToken;
        this.LogSkippedOperation(nameof(GatherConsentAsync));
        return Task.FromResult(UnsupportedGatheringResult);
    }

    public Task ShowPrivacyOptionsFormAsync(CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        this.LogSkippedOperation(nameof(ShowPrivacyOptionsFormAsync));
        return Task.CompletedTask;
    }

    public Task LoadAndShowConsentFormIfRequiredAsync(CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        this.LogSkippedOperation(nameof(LoadAndShowConsentFormIfRequiredAsync));
        return Task.CompletedTask;
    }

    public void ResetConsentForTesting()
    {
        this.LogSkippedOperation(nameof(ResetConsentForTesting));
    }

    private void LogSkippedOperation(string operation)
    {
        if (this.logger.IsEnabled(LogLevel.Debug))
        {
            this.logger.LogDebug(
                "Skipping AdMob consent operation {Operation} because it is not supported on {PlatformName}.",
                operation,
                PlatformName);
        }
    }

    #endregion
}

#endif
