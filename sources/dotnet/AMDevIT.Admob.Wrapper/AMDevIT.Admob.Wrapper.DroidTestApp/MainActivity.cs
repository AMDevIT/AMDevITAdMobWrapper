using AMDevIT.Admob.Wrapper.Ads;
using AMDevIT.Admob.Wrapper.Listeners;

namespace AMDevIT.Admob.Wrapper.DroidTestApp;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity
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
    private InterstitialAdWrapper? interstitialWrapper;
    private RewardedAdWrapper? rewardedWrapper;
    private AppOpenAdWrapper? appOpenWrapper;

    private TextView? statusText;
    private Button? btnInterstitial;
    private Button? btnRewarded;
    private Button? btnAppOpen;

    #endregion

    #region Methods

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_main);

        this.statusText = FindViewById<TextView>(Resource.Id.statusText)!;
        this.btnInterstitial = FindViewById<Button>(Resource.Id.btnInterstitial)!;
        this.btnRewarded = FindViewById<Button>(Resource.Id.btnRewarded)!;
        this.btnAppOpen = FindViewById<Button>(Resource.Id.btnAppOpen)!;

        this.btnInterstitial.Enabled = false;
        this.btnRewarded.Enabled = false;
        this.btnAppOpen.Enabled = false;

        this.btnInterstitial.Click += OnInterstitialClick;
        this.btnRewarded.Click += OnRewardedClick;
        this.btnAppOpen.Click += OnAppOpenClick;

        AdMobManager.Instance.Initialize(this, new InitListener(this));
    }

    private void OnAdMobInitialized()
    {
        RunOnUiThread(() =>
        {
            this.statusText!.Text = "AdMob initialized";
            this.btnInterstitial!.Enabled = true;
            this.btnRewarded!.Enabled = true;
            this.btnAppOpen!.Enabled = true;

            LoadBanner();
            PreloadInterstitial();
            PreloadRewarded();
            PreloadAppOpen();
        });
    }

    private void LoadBanner()
    {
        var container = FindViewById<FrameLayout>(Resource.Id.bannerContainer)!;
        this.bannerWrapper = new BannerAdWrapper(this);
        var adView = this.bannerWrapper.Load(BannerAdUnitId,
                                             new AdLoadListener(this, "Banner"));
        container.AddView(adView);
    }

    private void PreloadInterstitial()
    {
        this.interstitialWrapper = new InterstitialAdWrapper(this);
        this.interstitialWrapper.Load(InterstitialAdUnitId,
                                      new AdLoadListener(this, "Interstitial"),
                                      new AdEventListener(this, "Interstitial"));
    }

    private void PreloadRewarded()
    {
        this.rewardedWrapper = new RewardedAdWrapper(this);
        this.rewardedWrapper.Load(RewardedAdUnitId,
                                  new AdLoadListener(this, "Rewarded"),
                                  new AdEventListener(this, "Rewarded"));
    }

    private void PreloadAppOpen()
    {
        this.appOpenWrapper = new AppOpenAdWrapper(this);
        this.appOpenWrapper.Load(AppOpenAdUnitId,
                                 new AdLoadListener(this, "AppOpen"),
                                 new AdEventListener(this, "AppOpen"));
    }

    private void OnInterstitialClick(object? sender, EventArgs e)
    {
        if (this.interstitialWrapper?.IsLoaded == true)
            this.interstitialWrapper.Show(this, new AdLoadListener(this, "Interstitial show"));
        else
            UpdateStatus("Interstitial not ready yet");
    }

    private void OnRewardedClick(object? sender, EventArgs e)
    {
        if (this.rewardedWrapper?.IsLoaded == true)
            this.rewardedWrapper.Show(this, new RewardListener(this));
        else
            UpdateStatus("Rewarded not ready yet");
    }

    private void OnAppOpenClick(object? sender, EventArgs e)
    {
        if (this.appOpenWrapper?.IsLoaded == true)
            this.appOpenWrapper.Show(this, new AdLoadListener(this, "AppOpen show"));
        else
            UpdateStatus("AppOpen not ready yet");
    }

    public void UpdateStatus(string message)
    {
        RunOnUiThread(() => this.statusText!.Text = message);
    }

    protected override void OnDestroy()
    {
        this.bannerWrapper?.Destroy();
        base.OnDestroy();
    }

    #endregion

    #region Nested listeners classes

    private class InitListener(MainActivity activity)
        : Java.Lang.Object, IOnInitializedListener
    {
        private readonly MainActivity activity = activity;

        public void OnInitialized() =>
            this.activity.OnAdMobInitialized();

        public void OnInitializationFailed(string error) =>
            this.activity.UpdateStatus($"Init failed: {error}");
    }

    private class AdLoadListener(MainActivity activity, string tag) 
        : Java.Lang.Object, IOnAdLoadedListener
    {
        private readonly MainActivity activity = activity;
        private readonly string tag = tag;

        public void OnAdLoaded() =>
            this.activity.UpdateStatus($"{this.tag}: loaded ✓");

        public void OnAdFailedToLoad(int errorCode, string errorMessage) =>
            this.activity.UpdateStatus($"{this.tag}: failed {errorCode} - {errorMessage}");
    }

    private class AdEventListener(MainActivity activity, string tag) 
        : Java.Lang.Object, IOnAdEventListener
    {
        private readonly MainActivity activity = activity;
        private readonly string tag = tag;

        public void OnAdShown() => this.activity.UpdateStatus($"{this.tag}: shown");
        public void OnAdDismissed() => this.activity.UpdateStatus($"{this.tag}: dismissed");
        public void OnAdClicked() => this.activity.UpdateStatus($"{this.tag}: clicked");
        public void OnAdImpression() => this.activity.UpdateStatus($"{this.tag}: impression");

        public void OnAdFailedToShow(int errorCode, string errorMessage) =>
            this.activity.UpdateStatus($"{this.tag}: show failed {errorCode} - {errorMessage}");
    }

    private class RewardListener(MainActivity activity) 
        : Java.Lang.Object, IOnRewardEarnedListener
    {
        private readonly MainActivity activity = activity;

        public void OnRewardEarned(string type, int amount) =>
            this.activity.UpdateStatus($"Reward earned: {amount} {type}");
    }

    #endregion
}