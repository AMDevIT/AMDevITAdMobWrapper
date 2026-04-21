using Foundation;
using ObjCRuntime;
using UIKit;
 

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
		bool IsLoaded { get; }

		// -(BOOL)isShowing __attribute__((warn_unused_result("")));
		[Export ("isShowing")]
		bool IsShowing { get; }
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
		bool IsLoaded { get; }
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
		bool IsLoaded { get; }
	}
}
