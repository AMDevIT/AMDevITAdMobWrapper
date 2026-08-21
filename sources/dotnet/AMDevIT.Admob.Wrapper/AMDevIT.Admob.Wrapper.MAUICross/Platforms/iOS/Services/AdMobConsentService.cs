#if IOS

using AMDevIT.Admob.Wrapper.Extensions.iOSNative;
using AMDevIT.Admob.Wrapper.iOSNative;
using AMDevIT.Admob.Wrapper.MAUICross.Platforms.iOS.Diagnostics;
using Microsoft.Extensions.Logging;
using UIKit;
using ManagedAdMobAgeTreatment = AMDevIT.Admob.Wrapper.AdMobAgeTreatment;

namespace AMDevIT.Admob.Wrapper.MAUICross.Services;

public sealed class AdMobConsentService : IAdMobConsentService
{
    #region Fields

    private readonly IContextResolverService contextResolverService;
    private readonly AppleLoggerAdapter loggerAdapter;
    private readonly AdMobManager manager;

    #endregion

    #region Properties

    public bool IsSupported => true;

    public bool CanRequestAds => this.manager.CanRequestAds();

    public bool IsInitialized => this.manager.IsInitialized;

    public ConsentInformationSnapshot? CurrentConsentInformation =>
        this.manager.GetCurrentConsentInformation();

    #endregion

    #region .ctor

    public AdMobConsentService(ILogger<AdMobConsentService> logger,
                               IContextResolverService contextResolverService)
    {
        this.contextResolverService = contextResolverService;
        this.loggerAdapter = new AppleLoggerAdapter(logger);
        this.manager = new AdMobManager(this.loggerAdapter);
    }

    #endregion

    #region Methods

    public Task InitializeAsync(string applicationId,
                                CancellationToken cancellationToken = default)
    {
        _ = applicationId;
        return this.manager.InitializeAsync(this.GetViewController(), cancellationToken);
    }

    public Task InitializeAsync(string applicationId,
                                ManagedAdMobAgeTreatment ageTreatment,
                                CancellationToken cancellationToken = default)
    {
        _ = applicationId;
        return this.manager.InitializeAsync(this.GetViewController(),
                                            ageTreatment,
                                            cancellationToken);
    }

    public Task<ConsentInformationSnapshot> UpdateCurrentConsentInformationAsync(
        ConsentRequestOptions? options = null,
        CancellationToken cancellationToken = default) =>
        this.manager.UpdateCurrentConsentInformationAsync(this.GetViewController(),
                                                          options,
                                                          cancellationToken);

    public Task<ConsentGatheringResult> GatherConsentAsync(
        ConsentRequestOptions? options = null,
        CancellationToken cancellationToken = default) =>
        this.manager.GatherConsentAsync(this.GetViewController(), options, cancellationToken);

    public Task ShowPrivacyOptionsFormAsync(CancellationToken cancellationToken = default) =>
        this.manager.ShowPrivacyOptionsFormAsync(this.GetViewController(), cancellationToken);

    public Task LoadAndShowConsentFormIfRequiredAsync(
        CancellationToken cancellationToken = default) =>
        this.manager.LoadAndShowConsentFormIfRequiredAsync(this.GetViewController(),
                                                           cancellationToken);

    public void ResetConsentForTesting() => this.manager.ResetConsentForTesting();

    private UIViewController GetViewController() =>
        this.contextResolverService.GetViewController()
        ?? throw new InvalidOperationException("The current iOS view controller is not available.");

    #endregion
}

#endif
