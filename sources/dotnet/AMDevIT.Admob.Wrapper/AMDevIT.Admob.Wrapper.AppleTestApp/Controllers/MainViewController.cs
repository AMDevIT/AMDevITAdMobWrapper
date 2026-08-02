using AMDevIT.Admob.Wrapper.AppleTestApp.Diagnostics;
using AMDevIT.Admob.Wrapper.Extensions.iOSNative;
using AMDevIT.Admob.Wrapper.iOSNative;

namespace AMDevIT.Admob.Wrapper.AppleTestApp.Controllers;

[Register("MainViewController")]
public class MainViewController : UIViewController
{
    #region Const

    // Google test ad unit IDs
    private const string BannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";
    private const string InterstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
    private const string RewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";
    private const string AppOpenAdUnitId = "ca-app-pub-3940256099942544/9257395921";

    #endregion

    #region Fields

    private readonly ConsoleAppleLogger appleLogger;
    private readonly AdMobManager manager;
    private BannerAdWrapper? bannerWrapper;
    private UIView? bannerAdView;
    private InterstitialAdWrapper? interstitialWrapper;
    private RewardedAdWrapper? rewardedWrapper;
    private AppOpenAdWrapper? appOpenWrapper;
    private UILabel? statusLabel;
    private UIButton? btnInterstitial;
    private UIButton? btnRewarded;
    private UIButton? btnAppOpen;
    private UIButton? btnPrivacyOptions;
    private UIView? bannerContainer;
    private bool initializationStarted;

    #endregion

    #region .ctor

    public MainViewController()
    {
        this.appleLogger = new ConsoleAppleLogger();
        this.manager = new AdMobManager(this.appleLogger);
    }

    #endregion

    #region Methods

    public void UpdateStatus(string message)
    {
        this.InvokeOnMainThread(() => this.statusLabel!.Text = message);
    }

    public override void LoadView()
    {
        UIView rootView = new()
        {
            BackgroundColor = UIColor.SystemBackground
        };

        this.statusLabel = new UILabel
        {
            Lines = 0,
            Text = "Waiting for consent...",
            TextAlignment = UITextAlignment.Center
        };
        this.btnInterstitial = CreateButton("Show interstitial");
        this.btnRewarded = CreateButton("Show rewarded");
        this.btnAppOpen = CreateButton("Show app open");
        this.btnPrivacyOptions = CreateButton("Privacy options");
        this.bannerContainer = new UIView
        {
            BackgroundColor = UIColor.SecondarySystemBackground
        };

        this.btnInterstitial.Enabled = false;
        this.btnRewarded.Enabled = false;
        this.btnAppOpen.Enabled = false;
        this.btnPrivacyOptions.Enabled = false;
        this.btnPrivacyOptions.Hidden = true;

        this.btnInterstitial.TouchUpInside += OnInterstitialClick;
        this.btnRewarded.TouchUpInside += OnRewardedClick;
        this.btnAppOpen.TouchUpInside += OnAppOpenClick;
        this.btnPrivacyOptions.TouchUpInside += OnPrivacyOptionsClick;

        UIStackView stackView = new([
            this.statusLabel,
            this.btnInterstitial,
            this.btnRewarded,
            this.btnAppOpen,
            this.btnPrivacyOptions,
            this.bannerContainer
        ])
        {
            Axis = UILayoutConstraintAxis.Vertical,
            Spacing = 12,
            TranslatesAutoresizingMaskIntoConstraints = false
        };

        rootView.AddSubview(stackView);
        NSLayoutConstraint.ActivateConstraints([
            stackView.TopAnchor.ConstraintEqualTo(rootView.SafeAreaLayoutGuide.TopAnchor, 16),
            stackView.LeadingAnchor.ConstraintEqualTo(rootView.SafeAreaLayoutGuide.LeadingAnchor, 16),
            stackView.TrailingAnchor.ConstraintEqualTo(rootView.SafeAreaLayoutGuide.TrailingAnchor, -16),
            stackView.BottomAnchor.ConstraintLessThanOrEqualTo(
                rootView.SafeAreaLayoutGuide.BottomAnchor,
                -16),
            this.bannerContainer.HeightAnchor.ConstraintGreaterThanOrEqualTo(100)
        ]);

        this.Title = "AdMob iOS test";
        this.View = rootView;
    }

    public override void ViewDidAppear(bool animated)
    {
        base.ViewDidAppear(animated);

        if (this.initializationStarted)
            return;

        this.initializationStarted = true;
        _ = this.InitializeConsentAndAdsAsync();
    }

    private static UIButton CreateButton(string title)
    {
        UIButton button = UIButton.FromType(UIButtonType.System);
        button.SetTitle(title, UIControlState.Normal);
        return button;
    }

    private async Task InitializeConsentAndAdsAsync()
    {
        bool canRequestAds;

        try
        {
            this.UpdateStatus("Gathering consent...");
            ConsentGatheringResult result = await this.manager.GatherConsentAsync(this);
            canRequestAds = result.CanRequestAds;
            this.UpdatePrivacyOptionsAvailability(result.PrivacyOptionsRequired);
        }
        catch (ConsentException exception) when (exception.CanRequestAds == true)
        {
            this.UpdateStatus(
                $"Consent warning {exception.ErrorCode}; using the previous consent state.");
            canRequestAds = true;
            this.UpdatePrivacyOptionsAvailability(exception.PrivacyOptionsRequired == true);
        }
        catch (Exception exception)
        {
            this.UpdateStatus($"Consent failed: {exception.Message}");
            return;
        }

        if (!canRequestAds)
        {
            this.UpdateStatus("Consent completed, but ads cannot be requested.");
            return;
        }

        try
        {
            await this.manager.InitializeAsync(this);
            this.OnAdMobInitialized();
        }
        catch (Exception exception)
        {
            this.UpdateStatus($"AdMob initialization failed: {exception.Message}");
        }
    }

    private void OnAdMobInitialized()
    {
        this.InvokeOnMainThread(() =>
        {
            this.statusLabel!.Text = "AdMob initialized";
            this.btnInterstitial!.Enabled = true;
            this.btnRewarded!.Enabled = true;
            this.btnAppOpen!.Enabled = true;

            this.LoadBanner();
            this.PreloadInterstitial();
            this.PreloadRewarded();
            this.PreloadAppOpen();
        });
    }

    private void LoadBanner()
    {
        UIView container = this.bannerContainer!;
        this.View!.LayoutIfNeeded();

        nfloat availableWidth = container.Bounds.Width;
        if (availableWidth <= 0)
            availableWidth = (nfloat)Math.Max(1, (double)this.View.Bounds.Width - 32);

        this.bannerWrapper = new BannerAdWrapper(this.appleLogger);
        this.bannerAdView = this.bannerWrapper.LoadWithAdUnitId(
            BannerAdUnitId,
            this,
            BannerAdViewSize.Adaptive,
            availableWidth,
            new AdMobLoadListener(this, "Banner"),
            new AdMobEventListener(this, "Banner"));
        this.bannerAdView.TranslatesAutoresizingMaskIntoConstraints = false;
        container.AddSubview(this.bannerAdView);

        NSLayoutConstraint.ActivateConstraints([
            this.bannerAdView.CenterXAnchor.ConstraintEqualTo(container.CenterXAnchor),
            this.bannerAdView.TopAnchor.ConstraintEqualTo(container.TopAnchor),
            this.bannerAdView.WidthAnchor.ConstraintLessThanOrEqualTo(container.WidthAnchor)
        ]);
    }

    private void PreloadInterstitial()
    {
        this.interstitialWrapper = new InterstitialAdWrapper(this.appleLogger);
        this.interstitialWrapper.LoadWithAdUnitId(
            InterstitialAdUnitId,
            new AdMobLoadListener(this, "Interstitial"),
            new AdMobEventListener(this, "Interstitial"));
    }

    private void PreloadRewarded()
    {
        this.rewardedWrapper = new RewardedAdWrapper(this.appleLogger);
        this.rewardedWrapper.LoadWithAdUnitId(
            RewardedAdUnitId,
            new AdMobLoadListener(this, "Rewarded"),
            new AdMobEventListener(this, "Rewarded"));
    }

    private void PreloadAppOpen()
    {
        this.appOpenWrapper = new AppOpenAdWrapper(this.appleLogger);
        this.appOpenWrapper.LoadWithAdUnitId(
            AppOpenAdUnitId,
            new AdMobLoadListener(this, "AppOpen"),
            new AdMobEventListener(this, "AppOpen"));
    }

    private void UpdatePrivacyOptionsAvailability(bool required)
    {
        this.InvokeOnMainThread(() =>
        {
            this.btnPrivacyOptions!.Hidden = !required;
            this.btnPrivacyOptions.Enabled = required;
        });
    }

    #endregion

    private sealed class AdMobLoadListener(MainViewController viewController, string tag)
        : NSObject, IOnAdLoadedListener
    {
        #region Fields

        private readonly MainViewController viewController = viewController;
        private readonly string tag = tag;

        #endregion

        #region Methods

        public void OnAdLoaded() =>
            this.viewController.UpdateStatus($"{this.tag}: loaded");

        public void OnAdFailedToLoadWithErrorCode(nint errorCode, string errorMessage) =>
            this.viewController.UpdateStatus(
                $"{this.tag}: failed [{errorCode}] {errorMessage}");

        #endregion
    }

    private sealed class AdMobEventListener(MainViewController viewController, string tag)
        : NSObject, IOnAdEventListener
    {
        #region Fields

        private readonly MainViewController viewController = viewController;
        private readonly string tag = tag;

        #endregion

        #region Methods

        public void OnAdShown() =>
            this.viewController.UpdateStatus($"{this.tag}: shown");

        public void OnAdDismissed() =>
            this.viewController.UpdateStatus($"{this.tag}: dismissed");

        public void OnAdClicked() =>
            this.viewController.UpdateStatus($"{this.tag}: clicked");

        public void OnAdImpression() =>
            this.viewController.UpdateStatus($"{this.tag}: impression");

        public void OnAdFailedToShowWithErrorCode(nint errorCode, string errorMessage) =>
            this.viewController.UpdateStatus(
                $"{this.tag}: show failed [{errorCode}] {errorMessage}");

        #endregion
    }

    private sealed class RewardListener(MainViewController viewController)
        : NSObject, IOnRewardEarnedListener
    {
        #region Fields

        private readonly MainViewController viewController = viewController;

        #endregion

        #region Methods

        public void Amount(string type, nint amount) =>
            this.viewController.UpdateStatus($"Reward earned: {amount} {type}");

        #endregion
    }

    #region Event handlers

    private void OnInterstitialClick(object? sender, EventArgs e)
    {
        _ = sender;
        _ = e;

        if (this.interstitialWrapper?.IsLoaded == true)
            this.interstitialWrapper.ShowWithViewController(this);
        else
            this.UpdateStatus("Interstitial not ready yet");
    }

    private void OnRewardedClick(object? sender, EventArgs e)
    {
        _ = sender;
        _ = e;

        if (this.rewardedWrapper?.IsLoaded == true)
            this.rewardedWrapper.ShowWithViewController(this, new RewardListener(this));
        else
            this.UpdateStatus("Rewarded not ready yet");
    }

    private void OnAppOpenClick(object? sender, EventArgs e)
    {
        _ = sender;
        _ = e;

        if (this.appOpenWrapper?.IsLoaded == true &&
            this.appOpenWrapper.IsShowing == false)
        {
            this.appOpenWrapper.ShowWithViewController(this);
        }
        else
        {
            this.UpdateStatus("AppOpen not ready yet");
        }
    }

    private async void OnPrivacyOptionsClick(object? sender, EventArgs e)
    {
        _ = sender;
        _ = e;

        try
        {
            this.UpdateStatus("Opening privacy options...");
            await this.manager.ShowPrivacyOptionsFormAsync(this);
            ConsentInformationSnapshot? snapshot = this.manager.GetCurrentConsentInformation();
            this.UpdatePrivacyOptionsAvailability(
                snapshot?.PrivacyOptionsRequirementStatus ==
                PrivacyOptionsRequirementStatus.Required);
            this.UpdateStatus("Privacy options closed");
        }
        catch (Exception exception)
        {
            this.UpdateStatus($"Privacy options failed: {exception.Message}");
        }
    }

    #endregion
}
