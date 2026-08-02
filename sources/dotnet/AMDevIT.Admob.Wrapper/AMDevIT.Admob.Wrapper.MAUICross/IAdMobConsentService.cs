namespace AMDevIT.Admob.Wrapper.MAUICross;

public interface IAdMobConsentService
{
    #region Properties

    bool IsSupported { get; }

    bool CanRequestAds { get; }

    bool IsInitialized { get; }

    ConsentInformationSnapshot? CurrentConsentInformation { get; }

    #endregion

    #region Methods

    Task InitializeAsync(string applicationId, CancellationToken cancellationToken = default);

    Task<ConsentInformationSnapshot> UpdateCurrentConsentInformationAsync(
        ConsentRequestOptions? options = null,
        CancellationToken cancellationToken = default);

    Task<ConsentGatheringResult> GatherConsentAsync(
        ConsentRequestOptions? options = null,
        CancellationToken cancellationToken = default);

    Task ShowPrivacyOptionsFormAsync(CancellationToken cancellationToken = default);

    Task LoadAndShowConsentFormIfRequiredAsync(CancellationToken cancellationToken = default);

    void ResetConsentForTesting();

    #endregion
}
