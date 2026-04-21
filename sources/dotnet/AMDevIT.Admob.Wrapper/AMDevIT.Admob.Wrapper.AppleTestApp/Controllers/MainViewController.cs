using AMDevIT.Admob.Wrapper.iOSNative;
using CoreAudioKit;
using System.Diagnostics.Tracing;

namespace AMDevIT.Admob.Wrapper.AppleTestApp.Controllers;

[Register ("MainViewController")]
public class MainViewController 
	: UIViewController 
{
    #region Consts 

    // Test Ad Unit IDs 
    private const string BannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";
    private const string InterstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
    private const string RewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";
    private const string AppOpenAdUnitId = "ca-app-pub-3940256099942544/9257395921";

    #endregion

    #region Fields

    private BannerAdWrapper? bannerWrapper;
    private UIView? bannerAdView;
    private InterstitialAdWrapper? interstitialWrapper;
    private RewardedAdWrapper? rewardedWrapper;
    private AppOpenAdWrapper? appOpenWrapper;

    #endregion

    #region Properties

    [Outlet]
    UILabel? StatusLabel { get; set; }

    [Outlet]
    UIButton? BtnInterstitial { get; set; }

    [Outlet]
    UIButton? BtnRewarded { get; set; }

    [Outlet]
    UIButton? BtnAppOpen { get; set; }

    [Outlet]
    UIView? BannerContainer { get; set; }

    #endregion

    #region Methods

    public override void ViewDidLoad ()
	{
        base.ViewDidLoad();

        this.BtnInterstitial!.Enabled = false;
        this.BtnRewarded!.Enabled = false;
        this.BtnAppOpen!.Enabled = false;

        this.BtnInterstitial.TouchUpInside += OnInterstitialClick;
        this.BtnRewarded.TouchUpInside += OnRewardedClick;
        this.BtnAppOpen.TouchUpInside += OnAppOpenClick;

        AdMobManager.Instance.InitializeWithViewController(this, new AdMobInitListener(this));
    }

    private void OnAdMobInitialized()
    {
        this.UpdateStatus("AdMob initialized");
        this.BtnInterstitial!.Enabled = true;
        this.BtnRewarded!.Enabled = true;
        this.BtnAppOpen!.Enabled = true;

        this.LoadBanner();
        this.PreloadInterstitial();
        this.PreloadRewarded();
        this.PreloadAppOpen();
    }

    private void LoadBanner()
    {
        this.bannerWrapper = new BannerAdWrapper();
        var adView = this.bannerWrapper.LoadWithAdUnitId(BannerAdUnitId,
                                                         this,
                                                         new AdMobLoadListener(this, "Banner"),
                                                         null);

        adView.TranslatesAutoresizingMaskIntoConstraints = false;
        this.View!.AddSubview(adView);

        NSLayoutConstraint.ActivateConstraints(
        [
            adView.CenterXAnchor.ConstraintEqualTo(this.View.CenterXAnchor),
            adView.BottomAnchor.ConstraintEqualTo(this.View.SafeAreaLayoutGuide.BottomAnchor)
        ]);
    }

    private void PreloadInterstitial()
    {
        this.interstitialWrapper = new InterstitialAdWrapper();
        this.interstitialWrapper.LoadWithAdUnitId(InterstitialAdUnitId,
                                                  new AdMobLoadListener(this, "Interstitial"),
                                                  new AdMobEventListener(this, "Interstitial"));
    }

    private void PreloadRewarded()
    {
        this.rewardedWrapper = new RewardedAdWrapper();
        this.rewardedWrapper.LoadWithAdUnitId(RewardedAdUnitId,
                                              new AdMobLoadListener(this, "Rewarded"),
                                              new AdMobEventListener(this, "Rewarded"));
    }

    private void PreloadAppOpen()
    {
        this.appOpenWrapper = new AppOpenAdWrapper();
        this.appOpenWrapper.LoadWithAdUnitId(AppOpenAdUnitId,
                                             new AdMobLoadListener(this, "AppOpen"),
                                             new AdMobEventListener(this, "AppOpen"));
    }

    public void UpdateStatus(string message)
    {
        this.InvokeOnMainThread(() => this.StatusLabel!.Text = message);
    }

    private void OnInterstitialClick(object? sender, EventArgs e)
    {
        if (this.interstitialWrapper?.IsLoaded == true)
            this.interstitialWrapper.ShowWithViewController(this);
        else
            this.UpdateStatus("Interstitial not ready yet");
    }

    private void OnRewardedClick(object? sender, EventArgs e)
    {
        if (this.rewardedWrapper?.IsLoaded == true)
            this.rewardedWrapper.ShowWithViewController(this, new RewardListener(this));
        else
            this.UpdateStatus("Rewarded not ready yet");
    }

    private void OnAppOpenClick(object? sender, EventArgs e)
    {
        if (this.appOpenWrapper?.IsLoaded == true && this.appOpenWrapper?.IsShowing == false)
            this.appOpenWrapper.ShowWithViewController(this);
        else
            this.UpdateStatus("AppOpen not ready yet");
    }

    #endregion

    #region Nested classes

    private class AdMobInitListener(MainViewController vc) 
        : NSObject, IOnInitializedListener
    {
        private readonly MainViewController viewController = vc;

        public void OnInitialized() =>
            this.viewController.InvokeOnMainThread(() => this.viewController.OnAdMobInitialized());

        public void OnInitializationFailedWithError(string error) =>
            Console.WriteLine($"AdMob init failed: {error}");
    }

    private class AdMobLoadListener(MainViewController vc) 
        : NSObject, IOnAdLoadedListener
    {
        private readonly MainViewController viewController = vc;
        public void OnAdLoaded() =>
            Console.WriteLine("Banner loaded");

        public void OnAdFailedToLoadWithErrorCode(nint errorCode, string errorMessage) =>
           Console.WriteLine($"Banner failed: [{errorCode}] {errorMessage}");
    }

    private class AdMobEventListener(MainViewController vc, string tag) 
        : NSObject, IOnAdEventListener
    {
        private readonly MainViewController viewController = vc;
        private readonly string tag = tag;

        public void OnAdShown() => this.viewController.UpdateStatus($"{this.tag}: shown");
        public void OnAdDismissed() => this.viewController.UpdateStatus($"{this.tag}: dismissed");
        public void OnAdClicked() => this.viewController.UpdateStatus($"{this.tag}: clicked");
        public void OnAdImpression() => this.viewController.UpdateStatus($"{this.tag}: impression");

        public void OnAdFailedToShowWithErrorCode(nint errorCode, string errorMessage) =>
            this.viewController.UpdateStatus($"{this.tag}: show failed [{errorCode}] {errorMessage}");
    }

    private class RewardListener(MainViewController vc) 
        : NSObject, IOnRewardEarnedListener
    {
        private readonly MainViewController viewController = vc;

        public void Amount(string type, nint amount) =>
            this.viewController.UpdateStatus($"Reward earned: {amount} {type}");
    }

    #endregion
}

