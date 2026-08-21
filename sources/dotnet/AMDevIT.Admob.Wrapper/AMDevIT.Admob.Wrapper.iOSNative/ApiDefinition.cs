using Foundation;
using ObjCRuntime;
using UIKit;

namespace AMDevIT.Admob.Wrapper.iOSNative
{
    [BaseType(typeof(NSObject), Name = "_TtC12AdMobWrapper12AdMobManager")]
    [DisableDefaultCtor]
    interface AdMobManager
    {
        [Static]
        [Export("instance", ArgumentSemantic.Strong)]
        AdMobManager Instance { get; }

        [Export("initWithLogger:")]
        NativeHandle Constructor([NullAllowed] IAppleLogger logger);

        [Export("initializeWithViewController:listener:")]
        void InitializeWithViewController(UIViewController viewController,
                                          IOnInitializedListener listener);

        [Export("initializeWithViewController:ageTreatment:listener:")]
        void InitializeWithViewController(UIViewController viewController,
                                          AdMobAgeTreatment ageTreatment,
                                          IOnInitializedListener listener);

        [Export("isInitialized")]
        bool IsInitialized { get; }

        [Export("updateCurrentConsentInformationWithViewController:tagForUnderAgeOfConsent:listener:")]
        void UpdateCurrentConsentInformationWithViewController(
            UIViewController viewController,
            bool tagForUnderAgeOfConsent,
            IOnConsentInformationRequestListener listener);

        [Export("updateCurrentConsentInformationWithViewController:tagForUnderAgeOfConsent:listener:requestDebugParameters:")]
        void UpdateCurrentConsentInformationWithViewController(
            UIViewController viewController,
            bool tagForUnderAgeOfConsent,
            IOnConsentInformationRequestListener listener,
            [NullAllowed] ConsentInformationRequestDebugParameters requestDebugParameters);

        [return: NullAllowed]
        [Export("currentConsentInformation")]
        ConsentStatusData CurrentConsentInformation();

        [Export("showPrivacyOptionsFormWithViewController:listener:")]
        void ShowPrivacyOptionsFormWithViewController(UIViewController viewController,
                                                      IOnConsentFormEventListener listener);

        [Export("loadAndShowConsentFormIfRequiredWithViewController:listener:")]
        void LoadAndShowConsentFormIfRequiredWithViewController(
            UIViewController viewController,
            IOnConsentFormEventListener listener);

        [Export("canRequestAds")]
        bool CanRequestAds();

        [Export("gatherConsentWithViewController:tagForUnderAgeOfConsent:listener:")]
        void GatherConsentWithViewController(UIViewController viewController,
                                             bool tagForUnderAgeOfConsent,
                                             IOnConsentGatheringListener listener);

        [Export("gatherConsentWithViewController:tagForUnderAgeOfConsent:listener:requestDebugParameters:")]
        void GatherConsentWithViewController(
            UIViewController viewController,
            bool tagForUnderAgeOfConsent,
            IOnConsentGatheringListener listener,
            [NullAllowed] ConsentInformationRequestDebugParameters requestDebugParameters);

        [Export("resetConsentForTesting")]
        void ResetConsentForTesting();
    }

    [BaseType(typeof(NSObject), Name = "_TtC12AdMobWrapper16AppOpenAdWrapper")]
    interface AppOpenAdWrapper
    {
        [Export("initWithLogger:")]
        NativeHandle Constructor([NullAllowed] IAppleLogger logger);

        [Export("loadWithAdUnitId:loadListener:eventListener:")]
        void LoadWithAdUnitId(string adUnitId,
                              IOnAdLoadedListener loadListener,
                              [NullAllowed] IOnAdEventListener eventListener);

        [Export("showWithViewController:")]
        void ShowWithViewController(UIViewController viewController);

        [Export("isLoaded")]
        bool IsLoaded { get; }

        [Export("isShowing")]
        bool IsShowing { get; }
    }

    [BaseType(typeof(NSObject), Name = "_TtC12AdMobWrapper15BannerAdWrapper")]
    interface BannerAdWrapper
    {
        [Export("initWithLogger:")]
        NativeHandle Constructor([NullAllowed] IAppleLogger logger);

        [Export("loadWithAdUnitId:viewController:loadListener:eventListener:")]
        UIView LoadWithAdUnitId(string adUnitId,
                                UIViewController viewController,
                                IOnAdLoadedListener loadListener,
                                [NullAllowed] IOnAdEventListener eventListener);

        [Export("loadWithAdUnitId:viewController:adSize:adWidth:loadListener:eventListener:")]
        UIView LoadWithAdUnitId(string adUnitId,
                                UIViewController viewController,
                                BannerAdViewSize adSize,
                                nfloat adWidth,
                                IOnAdLoadedListener loadListener,
                                [NullAllowed] IOnAdEventListener eventListener);

        [Export("destroy")]
        void Destroy();
    }

    [BaseType(typeof(NSObject), Name = "_TtC12AdMobWrapper40ConsentInformationRequestDebugParameters")]
    interface ConsentInformationRequestDebugParameters
    {
        [NullAllowed]
        [Export("debugGeography", ArgumentSemantic.Strong)]
        NSNumber DebugGeography { get; }

        [NullAllowed]
        [Export("testDeviceHashedId")]
        string TestDeviceHashedId { get; }

        [Export("initWithDebugGeography:testDeviceHashedId:")]
        NativeHandle Constructor([NullAllowed] NSNumber debugGeography,
                                 [NullAllowed] string testDeviceHashedId);
    }

    [BaseType(typeof(NSObject), Name = "_TtC12AdMobWrapper17ConsentStatusData")]
    [DisableDefaultCtor]
    interface ConsentStatusData
    {
        [Export("lastRefreshTimestampMilliseconds")]
        long LastRefreshTimestampMilliseconds { get; }

        [Export("consentStatus")]
        nint ConsentStatus { get; }

        [Export("privacyOptionsRequirementStatus")]
        nint PrivacyOptionsRequirementStatus { get; }

        [Export("initWithLastRefreshTimestampMilliseconds:consentStatus:privacyOptionsRequirementStatus:")]
        NativeHandle Constructor(long lastRefreshTimestampMilliseconds,
                                 nint consentStatus,
                                 nint privacyOptionsRequirementStatus);
    }

    [BaseType(typeof(NSObject), Name = "_TtC12AdMobWrapper21InterstitialAdWrapper")]
    interface InterstitialAdWrapper
    {
        [Export("initWithLogger:")]
        NativeHandle Constructor([NullAllowed] IAppleLogger logger);

        [Export("loadWithAdUnitId:loadListener:eventListener:")]
        void LoadWithAdUnitId(string adUnitId,
                              IOnAdLoadedListener loadListener,
                              [NullAllowed] IOnAdEventListener eventListener);

        [Export("showWithViewController:")]
        void ShowWithViewController(UIViewController viewController);

        [Export("isLoaded")]
        bool IsLoaded { get; }
    }

    [BaseType(typeof(NSObject), Name = "_TtC12AdMobWrapper17RewardedAdWrapper")]
    interface RewardedAdWrapper
    {
        [Export("initWithLogger:")]
        NativeHandle Constructor([NullAllowed] IAppleLogger logger);

        [Export("loadWithAdUnitId:loadListener:eventListener:")]
        void LoadWithAdUnitId(string adUnitId,
                              IOnAdLoadedListener loadListener,
                              [NullAllowed] IOnAdEventListener eventListener);

        [Export("showWithViewController:rewardListener:")]
        void ShowWithViewController(UIViewController viewController,
                                    IOnRewardEarnedListener rewardListener);

        [Export("isLoaded")]
        bool IsLoaded { get; }
    }

    [Protocol(Name = "_TtP12AdMobWrapper12IAppleLogger_")]
    interface AppleLogger
    {
        [Abstract]
        [Export("isEnabledWithLevel:")]
        bool IsEnabled(AppleLogLevel level);

        [Abstract]
        [Export("logTraceWithMessage:tag:")]
        void LogTrace(string message, [NullAllowed] string tag);

        [Abstract]
        [Export("logDebugWithMessage:tag:")]
        void LogDebug(string message, [NullAllowed] string tag);

        [Abstract]
        [Export("logInfoWithMessage:tag:")]
        void LogInfo(string message, [NullAllowed] string tag);

        [Abstract]
        [Export("logWarningWithMessage:tag:")]
        void LogWarning(string message, [NullAllowed] string tag);

        [Abstract]
        [Export("logErrorWithMessage:tag:")]
        void LogError(string message, [NullAllowed] string tag);

        [Abstract]
        [Export("logCriticalWithMessage:tag:")]
        void LogCritical(string message, [NullAllowed] string tag);
    }

    [Protocol(Name = "_TtP12AdMobWrapper17OnAdEventListener_")]
    interface OnAdEventListener
    {
        [Abstract]
        [Export("onAdShown")]
        void OnAdShown();

        [Abstract]
        [Export("onAdDismissed")]
        void OnAdDismissed();

        [Abstract]
        [Export("onAdClicked")]
        void OnAdClicked();

        [Abstract]
        [Export("onAdImpression")]
        void OnAdImpression();

        [Abstract]
        [Export("onAdFailedToShowWithErrorCode:errorMessage:")]
        void OnAdFailedToShowWithErrorCode(nint errorCode, string errorMessage);
    }

    [Protocol(Name = "_TtP12AdMobWrapper18OnAdLoadedListener_")]
    interface OnAdLoadedListener
    {
        [Abstract]
        [Export("onAdLoaded")]
        void OnAdLoaded();

        [Abstract]
        [Export("onAdFailedToLoadWithErrorCode:errorMessage:")]
        void OnAdFailedToLoadWithErrorCode(nint errorCode, string errorMessage);
    }

    [Protocol(Name = "_TtP12AdMobWrapper26OnConsentFormEventListener_")]
    interface OnConsentFormEventListener
    {
        [Abstract]
        [Export("onDismissed")]
        void OnDismissed();

        [Abstract]
        [Export("onDismissedWithErrorWithErrorCode:errorMessage:")]
        void OnDismissedWithError(nint errorCode, [NullAllowed] string errorMessage);
    }

    [Protocol(Name = "_TtP12AdMobWrapper26OnConsentGatheringListener_")]
    interface OnConsentGatheringListener
    {
        [Abstract]
        [Export("onCompletedWithCanRequestAds:privacyOptionsRequired:")]
        void OnCompleted(bool canRequestAds, bool privacyOptionsRequired);

        [Abstract]
        [Export("onCompletedWithErrorWithErrorCode:errorMessage:canRequestAds:privacyOptionsRequired:")]
        void OnCompletedWithError(nint errorCode,
                                  string errorMessage,
                                  bool canRequestAds,
                                  bool privacyOptionsRequired);
    }

    [Protocol(Name = "_TtP12AdMobWrapper35OnConsentInformationRequestListener_")]
    interface OnConsentInformationRequestListener
    {
        [Abstract]
        [Export("onConsentInformationRequestSuccess")]
        void OnConsentInformationRequestSuccess();

        [Abstract]
        [Export("onConsentInformationRequestFailureWithErrorCode:errorMessage:")]
        void OnConsentInformationRequestFailure(nint errorCode, string errorMessage);
    }

    [Protocol(Name = "_TtP12AdMobWrapper21OnInitializedListener_")]
    interface OnInitializedListener
    {
        [Abstract]
        [Export("onInitialized")]
        void OnInitialized();

        [Abstract]
        [Export("onInitializationFailedWithError:")]
        void OnInitializationFailedWithError(string error);
    }

    [Protocol(Name = "_TtP12AdMobWrapper22OnRewardEarnedListener_")]
    interface OnRewardEarnedListener
    {
        [Abstract]
        [Export("onRewardEarnedWithType:amount:")]
        void Amount(string type, nint amount);
    }
}
