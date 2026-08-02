using System;
using AdMobWrapper;
using Foundation;
using ObjCRuntime;

namespace AMDevIT.Admob.Wrapper
{
	// @interface AdMobManager : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC12AdMobWrapper12AdMobManager")]
	[DisableDefaultCtor]
	interface AdMobManager
	{
		// @property (readonly, nonatomic, strong, class) AdMobManager * _Nonnull instance;
		[Static]
		[Export ("instance", ArgumentSemantic.Strong)]
		AdMobManager Instance { get; }

		// -(void)initializeWithViewController:(UIViewController * _Nonnull)viewController listener:(id<OnInitializedListener> _Nonnull)listener;
		[Export ("initializeWithViewController:listener:")]
		void InitializeWithViewController (UIViewController viewController, OnInitializedListener listener);

		// -(BOOL)isInitialized __attribute__((warn_unused_result("")));
		[Export ("isInitialized")]
		[Verify (MethodToProperty)]
		bool IsInitialized { get; }
	}

	// @interface AppOpenAdWrapper : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC12AdMobWrapper16AppOpenAdWrapper")]
	interface AppOpenAdWrapper
	{
		// -(void)loadWithAdUnitId:(NSString * _Nonnull)adUnitId loadListener:(id<OnAdLoadedListener> _Nonnull)loadListener eventListener:(id<OnAdEventListener> _Nullable)eventListener;
		[Export ("loadWithAdUnitId:loadListener:eventListener:")]
		void LoadWithAdUnitId (string adUnitId, OnAdLoadedListener loadListener, [NullAllowed] OnAdEventListener eventListener);

		// -(void)showWithViewController:(UIViewController * _Nonnull)viewController;
		[Export ("showWithViewController:")]
		void ShowWithViewController (UIViewController viewController);

		// -(BOOL)isLoaded __attribute__((warn_unused_result("")));
		[Export ("isLoaded")]
		[Verify (MethodToProperty)]
		bool IsLoaded { get; }

		// -(BOOL)isShowing __attribute__((warn_unused_result("")));
		[Export ("isShowing")]
		[Verify (MethodToProperty)]
		bool IsShowing { get; }
	}

	// @interface AdMobWrapper_Swift_333 (AppOpenAdWrapper)
	[Category]
	[BaseType (typeof(AppOpenAdWrapper))]
	interface AppOpenAdWrapper_AdMobWrapper_Swift_333
	{
		// -(void)adDidRecordImpression:(id<GADFullScreenPresentingAd> _Nonnull)ad;
		[Export ("adDidRecordImpression:")]
		void AdDidRecordImpression (GADFullScreenPresentingAd ad);

		// -(void)adDidRecordClick:(id<GADFullScreenPresentingAd> _Nonnull)ad;
		[Export ("adDidRecordClick:")]
		void AdDidRecordClick (GADFullScreenPresentingAd ad);

		// -(void)adWillPresentFullScreenContent:(id<GADFullScreenPresentingAd> _Nonnull)ad;
		[Export ("adWillPresentFullScreenContent:")]
		void AdWillPresentFullScreenContent (GADFullScreenPresentingAd ad);

		// -(void)adWillDismissFullScreenContent:(id<GADFullScreenPresentingAd> _Nonnull)ad;
		[Export ("adWillDismissFullScreenContent:")]
		void AdWillDismissFullScreenContent (GADFullScreenPresentingAd ad);

		// -(void)adDidDismissFullScreenContent:(id<GADFullScreenPresentingAd> _Nonnull)ad;
		[Export ("adDidDismissFullScreenContent:")]
		void AdDidDismissFullScreenContent (GADFullScreenPresentingAd ad);

		// -(void)ad:(id<GADFullScreenPresentingAd> _Nonnull)ad didFailToPresentFullScreenContentWithError:(NSError * _Nonnull)error;
		[Export ("ad:didFailToPresentFullScreenContentWithError:")]
		void Ad (GADFullScreenPresentingAd ad, NSError error);
	}

	// @interface BannerAdWrapper : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC12AdMobWrapper15BannerAdWrapper")]
	interface BannerAdWrapper
	{
		// -(UIView * _Nonnull)loadWithAdUnitId:(NSString * _Nonnull)adUnitId viewController:(UIViewController * _Nonnull)viewController loadListener:(id<OnAdLoadedListener> _Nonnull)loadListener eventListener:(id<OnAdEventListener> _Nullable)eventListener __attribute__((warn_unused_result("")));
		[Export ("loadWithAdUnitId:viewController:loadListener:eventListener:")]
		UIView LoadWithAdUnitId (string adUnitId, UIViewController viewController, OnAdLoadedListener loadListener, [NullAllowed] OnAdEventListener eventListener);

		// -(void)destroy;
		[Export ("destroy")]
		void Destroy ();
	}

	// @interface AdMobWrapper_Swift_351 (BannerAdWrapper)
	[Category]
	[BaseType (typeof(BannerAdWrapper))]
	interface BannerAdWrapper_AdMobWrapper_Swift_351
	{
		// -(void)bannerViewDidReceiveAd:(GADBannerView * _Nonnull)bannerView;
		[Export ("bannerViewDidReceiveAd:")]
		void BannerViewDidReceiveAd (GADBannerView bannerView);

		// -(void)bannerView:(GADBannerView * _Nonnull)bannerView didFailToReceiveAdWithError:(NSError * _Nonnull)error;
		[Export ("bannerView:didFailToReceiveAdWithError:")]
		void BannerView (GADBannerView bannerView, NSError error);

		// -(void)bannerViewDidRecordImpression:(GADBannerView * _Nonnull)bannerView;
		[Export ("bannerViewDidRecordImpression:")]
		void BannerViewDidRecordImpression (GADBannerView bannerView);

		// -(void)bannerViewDidRecordClick:(GADBannerView * _Nonnull)bannerView;
		[Export ("bannerViewDidRecordClick:")]
		void BannerViewDidRecordClick (GADBannerView bannerView);

		// -(void)bannerViewWillPresentScreen:(GADBannerView * _Nonnull)bannerView;
		[Export ("bannerViewWillPresentScreen:")]
		void BannerViewWillPresentScreen (GADBannerView bannerView);

		// -(void)bannerViewWillDismissScreen:(GADBannerView * _Nonnull)bannerView;
		[Export ("bannerViewWillDismissScreen:")]
		void BannerViewWillDismissScreen (GADBannerView bannerView);
	}

	// @interface InterstitialAdWrapper : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC12AdMobWrapper21InterstitialAdWrapper")]
	interface InterstitialAdWrapper
	{
		// -(void)loadWithAdUnitId:(NSString * _Nonnull)adUnitId loadListener:(id<OnAdLoadedListener> _Nonnull)loadListener eventListener:(id<OnAdEventListener> _Nullable)eventListener;
		[Export ("loadWithAdUnitId:loadListener:eventListener:")]
		void LoadWithAdUnitId (string adUnitId, OnAdLoadedListener loadListener, [NullAllowed] OnAdEventListener eventListener);

		// -(void)showWithViewController:(UIViewController * _Nonnull)viewController;
		[Export ("showWithViewController:")]
		void ShowWithViewController (UIViewController viewController);

		// -(BOOL)isLoaded __attribute__((warn_unused_result("")));
		[Export ("isLoaded")]
		[Verify (MethodToProperty)]
		bool IsLoaded { get; }
	}

	// @interface AdMobWrapper_Swift_368 (InterstitialAdWrapper)
	[Category]
	[BaseType (typeof(InterstitialAdWrapper))]
	interface InterstitialAdWrapper_AdMobWrapper_Swift_368
	{
		// -(void)adDidRecordImpression:(id<GADFullScreenPresentingAd> _Nonnull)ad;
		[Export ("adDidRecordImpression:")]
		void AdDidRecordImpression (GADFullScreenPresentingAd ad);

		// -(void)adDidRecordClick:(id<GADFullScreenPresentingAd> _Nonnull)ad;
		[Export ("adDidRecordClick:")]
		void AdDidRecordClick (GADFullScreenPresentingAd ad);

		// -(void)adWillPresentFullScreenContent:(id<GADFullScreenPresentingAd> _Nonnull)ad;
		[Export ("adWillPresentFullScreenContent:")]
		void AdWillPresentFullScreenContent (GADFullScreenPresentingAd ad);

		// -(void)adWillDismissFullScreenContent:(id<GADFullScreenPresentingAd> _Nonnull)ad;
		[Export ("adWillDismissFullScreenContent:")]
		void AdWillDismissFullScreenContent (GADFullScreenPresentingAd ad);

		// -(void)ad:(id<GADFullScreenPresentingAd> _Nonnull)ad didFailToPresentFullScreenContentWithError:(NSError * _Nonnull)error;
		[Export ("ad:didFailToPresentFullScreenContentWithError:")]
		void Ad (GADFullScreenPresentingAd ad, NSError error);

		// -(void)adDidDismissFullScreenContent:(id<GADFullScreenPresentingAd> _Nonnull)ad;
		[Export ("adDidDismissFullScreenContent:")]
		void AdDidDismissFullScreenContent (GADFullScreenPresentingAd ad);
	}

	// @protocol OnAdEventListener
	/*
  Check whether adding [Model] to this declaration is appropriate.
  [Model] is used to generate a C# class that implements this protocol,
  and might be useful for protocols that consumers are supposed to implement,
  since consumers can subclass the generated class instead of implementing
  the generated interface. If consumers are not supposed to implement this
  protocol, then [Model] is redundant and will generate code that will never
  be used.
*/[Protocol (Name = "_TtP12AdMobWrapper17OnAdEventListener_")]
	interface OnAdEventListener
	{
		// @required -(void)onAdShown;
		[Abstract]
		[Export ("onAdShown")]
		void OnAdShown ();

		// @required -(void)onAdDismissed;
		[Abstract]
		[Export ("onAdDismissed")]
		void OnAdDismissed ();

		// @required -(void)onAdClicked;
		[Abstract]
		[Export ("onAdClicked")]
		void OnAdClicked ();

		// @required -(void)onAdImpression;
		[Abstract]
		[Export ("onAdImpression")]
		void OnAdImpression ();

		// @required -(void)onAdFailedToShowWithErrorCode:(NSInteger)errorCode errorMessage:(NSString * _Nonnull)errorMessage;
		[Abstract]
		[Export ("onAdFailedToShowWithErrorCode:errorMessage:")]
		void OnAdFailedToShowWithErrorCode (nint errorCode, string errorMessage);
	}

	// @protocol OnAdLoadedListener
	/*
  Check whether adding [Model] to this declaration is appropriate.
  [Model] is used to generate a C# class that implements this protocol,
  and might be useful for protocols that consumers are supposed to implement,
  since consumers can subclass the generated class instead of implementing
  the generated interface. If consumers are not supposed to implement this
  protocol, then [Model] is redundant and will generate code that will never
  be used.
*/[Protocol (Name = "_TtP12AdMobWrapper18OnAdLoadedListener_")]
	interface OnAdLoadedListener
	{
		// @required -(void)onAdLoaded;
		[Abstract]
		[Export ("onAdLoaded")]
		void OnAdLoaded ();

		// @required -(void)onAdFailedToLoadWithErrorCode:(NSInteger)errorCode errorMessage:(NSString * _Nonnull)errorMessage;
		[Abstract]
		[Export ("onAdFailedToLoadWithErrorCode:errorMessage:")]
		void OnAdFailedToLoadWithErrorCode (nint errorCode, string errorMessage);
	}

	// @protocol OnInitializedListener
	/*
  Check whether adding [Model] to this declaration is appropriate.
  [Model] is used to generate a C# class that implements this protocol,
  and might be useful for protocols that consumers are supposed to implement,
  since consumers can subclass the generated class instead of implementing
  the generated interface. If consumers are not supposed to implement this
  protocol, then [Model] is redundant and will generate code that will never
  be used.
*/[Protocol (Name = "_TtP12AdMobWrapper21OnInitializedListener_")]
	interface OnInitializedListener
	{
		// @required -(void)onInitialized;
		[Abstract]
		[Export ("onInitialized")]
		void OnInitialized ();

		// @required -(void)onInitializationFailedWithError:(NSString * _Nonnull)error;
		[Abstract]
		[Export ("onInitializationFailedWithError:")]
		void OnInitializationFailedWithError (string error);
	}

	// @protocol OnRewardEarnedListener
	/*
  Check whether adding [Model] to this declaration is appropriate.
  [Model] is used to generate a C# class that implements this protocol,
  and might be useful for protocols that consumers are supposed to implement,
  since consumers can subclass the generated class instead of implementing
  the generated interface. If consumers are not supposed to implement this
  protocol, then [Model] is redundant and will generate code that will never
  be used.
*/[Protocol (Name = "_TtP12AdMobWrapper22OnRewardEarnedListener_")]
	interface OnRewardEarnedListener
	{
		// @required -(void)onRewardEarnedWithType:(NSString * _Nonnull)type amount:(NSInteger)amount;
		[Abstract]
		[Export ("onRewardEarnedWithType:amount:")]
		void Amount (string type, nint amount);
	}

	// @interface RewardedAdWrapper : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC12AdMobWrapper17RewardedAdWrapper")]
	interface RewardedAdWrapper
	{
		// -(void)loadWithAdUnitId:(NSString * _Nonnull)adUnitId loadListener:(id<OnAdLoadedListener> _Nonnull)loadListener eventListener:(id<OnAdEventListener> _Nullable)eventListener;
		[Export ("loadWithAdUnitId:loadListener:eventListener:")]
		void LoadWithAdUnitId (string adUnitId, OnAdLoadedListener loadListener, [NullAllowed] OnAdEventListener eventListener);

		// -(void)showWithViewController:(UIViewController * _Nonnull)viewController rewardListener:(id<OnRewardEarnedListener> _Nonnull)rewardListener;
		[Export ("showWithViewController:rewardListener:")]
		void ShowWithViewController (UIViewController viewController, OnRewardEarnedListener rewardListener);

		// -(BOOL)isLoaded __attribute__((warn_unused_result("")));
		[Export ("isLoaded")]
		[Verify (MethodToProperty)]
		bool IsLoaded { get; }
	}

	// @interface AdMobWrapper_Swift_411 (RewardedAdWrapper)
	[Category]
	[BaseType (typeof(RewardedAdWrapper))]
	interface RewardedAdWrapper_AdMobWrapper_Swift_411
	{
		// -(void)adDidRecordImpression:(id<GADFullScreenPresentingAd> _Nonnull)ad;
		[Export ("adDidRecordImpression:")]
		void AdDidRecordImpression (GADFullScreenPresentingAd ad);

		// -(void)adDidRecordClick:(id<GADFullScreenPresentingAd> _Nonnull)ad;
		[Export ("adDidRecordClick:")]
		void AdDidRecordClick (GADFullScreenPresentingAd ad);

		// -(void)adWillPresentFullScreenContent:(id<GADFullScreenPresentingAd> _Nonnull)ad;
		[Export ("adWillPresentFullScreenContent:")]
		void AdWillPresentFullScreenContent (GADFullScreenPresentingAd ad);

		// -(void)adWillDismissFullScreenContent:(id<GADFullScreenPresentingAd> _Nonnull)ad;
		[Export ("adWillDismissFullScreenContent:")]
		void AdWillDismissFullScreenContent (GADFullScreenPresentingAd ad);

		// -(void)adDidDismissFullScreenContent:(id<GADFullScreenPresentingAd> _Nonnull)ad;
		[Export ("adDidDismissFullScreenContent:")]
		void AdDidDismissFullScreenContent (GADFullScreenPresentingAd ad);

		// -(void)ad:(id<GADFullScreenPresentingAd> _Nonnull)ad didFailToPresentFullScreenContentWithError:(NSError * _Nonnull)error;
		[Export ("ad:didFailToPresentFullScreenContentWithError:")]
		void Ad (GADFullScreenPresentingAd ad, NSError error);
	}
}
