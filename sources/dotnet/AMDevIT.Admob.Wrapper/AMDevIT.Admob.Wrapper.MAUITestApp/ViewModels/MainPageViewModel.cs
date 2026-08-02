using AMDevIT.Admob.Wrapper.MAUICross;
using AMDevIT.Admob.Wrapper.MAUITestApp.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace AMDevIT.Admob.Wrapper.MAUITestApp.ViewModels;

public class MainPageViewModel(ILogger<MainPageViewModel> logger,
                               IInterstitialAdService interstitialAdService,
                               IAdUnitProviderService adUnitProviderService,
#if ANDROID
                               IAdMobConsentService adMobConsentService,
#endif
                               IDispatcherService dispatcherService)
    : ViewModelBase(logger)
{
#if ANDROID
    #region Const

    private const string AndroidApplicationId = "ca-app-pub-3940256099942544~3347511713";

    #endregion
#endif

    #region Fields

    private bool showStatusMessage = false;
    private string? statusMessage = null;
    private AsyncRelayCommand? showInterstitialAdCommand;

    #endregion

    #region Properties

    protected IInterstitialAdService InterstitialAdService => interstitialAdService;
    protected IAdUnitProviderService AdUnitProviderService => adUnitProviderService;

#if ANDROID
    protected IAdMobConsentService AdMobConsentService => adMobConsentService;
#endif

    protected IDispatcherService DispatcherService => dispatcherService;

    public string? StatusMessage
    {
        get => this.statusMessage;
        set => this.SetProperty(ref this.statusMessage, value);
    }

    public bool ShowStatusMessage
    {
        get => this.showStatusMessage;
        set => this.SetProperty(ref this.showStatusMessage, value);
    }

    public AsyncRelayCommand ShowInterstitialAdCommand => this.showInterstitialAdCommand ??= new AsyncRelayCommand(this.ShowInterstitialAd);

    #endregion

    #region Methods

    public override void RegisterEvents()
    {
        base.RegisterEvents();

        this.InterstitialAdService.AdLoaded += InterstitialAdService_AdLoaded;
        this.InterstitialAdService.AdShown += InterstitialAdService_AdShown;
        this.InterstitialAdService.AdDismissed += InterstitialAdService_AdDismissed;
        this.InterstitialAdService.AdClicked += InterstitialAdService_AdClicked;
        this.InterstitialAdService.AdFailedToLoad += InterstitialAdService_AdFailedToLoad;
        this.InterstitialAdService.AdFailedToShow += InterstitialAdService_AdFailedToShow;
    }    

    public override void UnregisterEvents()
    {
        base.UnregisterEvents();


        this.InterstitialAdService.AdLoaded -= InterstitialAdService_AdLoaded;
        this.InterstitialAdService.AdShown -= InterstitialAdService_AdShown;
        this.InterstitialAdService.AdDismissed -= InterstitialAdService_AdDismissed;
        this.InterstitialAdService.AdClicked -= InterstitialAdService_AdClicked;
        this.InterstitialAdService.AdFailedToLoad -= InterstitialAdService_AdFailedToLoad;
        this.InterstitialAdService.AdFailedToShow -= InterstitialAdService_AdFailedToShow;
    }

    private async Task ShowInterstitialAd(CancellationToken cancellationToken = default)
    {
#if ANDROID
        await this.EnsureAndroidAdMobReadyAsync(cancellationToken);
#endif

        string interstitialAdUnitId = this.AdUnitProviderService.GetInterstitialAdUnitId();

        if (this.Logger.IsEnabled(LogLevel.Debug))
            this.Logger.LogDebug("Show interstitial ad using ad unit id: {AdUnitId}", interstitialAdUnitId);

        await this.InterstitialAdService.LoadAsync(interstitialAdUnitId, cancellationToken: cancellationToken);
        this.InterstitialAdService.Show();
    }

#if ANDROID
    private async Task EnsureAndroidAdMobReadyAsync(CancellationToken cancellationToken)
    {
        if (this.AdMobConsentService.IsInitialized)
            return;

        bool canRequestAds;

        try
        {
            ConsentGatheringResult result = await this.AdMobConsentService.GatherConsentAsync(
                cancellationToken: cancellationToken);
            canRequestAds = result.CanRequestAds;
        }
        catch (ConsentException exception) when (exception.CanRequestAds == true)
        {
            this.Logger.LogWarning(exception,
                                   "Consent gathering failed, but the previous consent state allows ads.");
            canRequestAds = true;
        }

        if (!canRequestAds)
            throw new InvalidOperationException("The current consent state does not allow ad requests.");

        await this.AdMobConsentService.InitializeAsync(AndroidApplicationId, cancellationToken);
    }
#endif

    #endregion

    #region Event handlers

    private void InterstitialAdService_AdFailedToShow(object? sender, AdFailedEventArgs e)
    {
        if (this.Logger.IsEnabled(LogLevel.Warning))
            this.Logger.LogWarning("Failed to show interstitial ad: {ErrorMessage} (Code: {ErrorCode})",
                                   e.ErrorMessage,
                                   e.ErrorCode);
    }

    private void InterstitialAdService_AdFailedToLoad(object? sender, AdFailedEventArgs e)
    {
        if (this.Logger.IsEnabled(LogLevel.Warning))
            this.Logger.LogWarning("Failed to load interstitial ad: {ErrorMessage} (Code: {ErrorCode})", 
                                   e.ErrorMessage, 
                                   e.ErrorCode);

        this.DispatcherService.TryEnqueue(() =>
        {
            this.StatusMessage = $"Failed to load interstitial ad: {e.ErrorMessage} (Code: {e.ErrorCode})";
            this.ShowStatusMessage = true;
        });
    }

    private void InterstitialAdService_AdClicked(object? sender, EventArgs e)
    {
        if (this.Logger.IsEnabled(LogLevel.Debug))
            this.Logger.LogDebug("Interstitial ad clicked");
    }

    private void InterstitialAdService_AdDismissed(object? sender, EventArgs e)
    {
        if (this.Logger.IsEnabled(LogLevel.Debug))
            this.Logger.LogDebug("Interstitial ad dismissed");
    }

    private void InterstitialAdService_AdShown(object? sender, EventArgs e)
    {
        if (this.Logger.IsEnabled(LogLevel.Debug))
            this.Logger.LogDebug("Interstitial ad shown");

        this.DispatcherService.TryEnqueue(() =>
        {
            this.ShowStatusMessage = false;
        });
    }

    private void InterstitialAdService_AdLoaded(object? sender, EventArgs e)
    {
        if (this.Logger.IsEnabled(LogLevel.Debug))
            this.Logger.LogDebug("Interstitial ad loaded");
    }

    #endregion
}
