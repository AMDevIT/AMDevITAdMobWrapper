#if ANDROID

using AMDevIT.Admob.Wrapper.Extensions.Droid;
using AMDevIT.Admob.Wrapper.MAUICross.Platforms.Android.Diagnostics;
using Android.App;
using Microsoft.Extensions.Logging;

namespace AMDevIT.Admob.Wrapper.MAUICross.Services;

public sealed class AdMobConsentService : IAdMobConsentService
{
    #region Fields

    private readonly IContextResolverService contextResolverService;
    private readonly DroidLoggerAdapter loggerAdapter;
    private readonly AdMobManager manager;

    #endregion

    #region Properties

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
        this.loggerAdapter = new DroidLoggerAdapter(logger);
        this.manager = new AdMobManager(this.loggerAdapter);
    }

    #endregion

    #region Methods

    public Task InitializeAsync(string applicationId, CancellationToken cancellationToken = default)
    {
        Activity activity = this.GetActivity();
        return this.manager.InitializeAsync(activity.ApplicationContext!,
                                            applicationId,
                                            cancellationToken);
    }

    public Task<ConsentInformationSnapshot> UpdateCurrentConsentInformationAsync(
        ConsentRequestOptions? options = null,
        CancellationToken cancellationToken = default) =>
        this.manager.UpdateCurrentConsentInformationAsync(this.GetActivity(),
                                                          options,
                                                          cancellationToken);

    public Task<ConsentGatheringResult> GatherConsentAsync(
        ConsentRequestOptions? options = null,
        CancellationToken cancellationToken = default) =>
        this.manager.GatherConsentAsync(this.GetActivity(), options, cancellationToken);

    public Task ShowPrivacyOptionsFormAsync(CancellationToken cancellationToken = default) =>
        this.manager.ShowPrivacyOptionsFormAsync(this.GetActivity(), cancellationToken);

    public Task LoadAndShowConsentFormIfRequiredAsync(CancellationToken cancellationToken = default) =>
        this.manager.LoadAndShowConsentFormIfRequiredAsync(this.GetActivity(), cancellationToken);

    public void ResetConsentForTesting() => this.manager.ResetConsentForTesting();

    private Activity GetActivity() =>
        this.contextResolverService.GetContext() as Activity
        ?? throw new InvalidOperationException("The current Android activity is not available.");

    #endregion
}

#endif
