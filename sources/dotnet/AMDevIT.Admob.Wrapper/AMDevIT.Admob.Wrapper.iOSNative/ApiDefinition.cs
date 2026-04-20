
using Foundation;
using ObjCRuntime;
using UIKit;

namespace AMDevIT.Admob.Wrapper.iOSNative
{

    // The first step to creating a binding is to add your native framework ("MyLibrary.xcframework")
    // to the project.
    // Open your binding csproj and add a section like this
    // <ItemGroup>
    //   <NativeReference Include="MyLibrary.xcframework">
    //     <Kind>Framework</Kind>
    //     <Frameworks></Frameworks>
    //   </NativeReference>
    // </ItemGroup>
    //
    // Once you've added it, you will need to customize it for your specific library:
    //  - Change the Include to the correct path/name of your library
    //  - Change Kind to Static (.a) or Framework (.framework/.xcframework) based upon the library kind and extension.
    //    - Dynamic (.dylib) is a third option but rarely if ever valid, and only on macOS and Mac Catalyst
    //  - If your library depends on other frameworks, add them inside <Frameworks></Frameworks>
    // Example:
    // <NativeReference Include="libs\MyTestFramework.xcframework">
    //   <Kind>Framework</Kind>
    //   <Frameworks>CoreLocation ModelIO</Frameworks>
    // </NativeReference>
    // 
    // Once you've done that, you're ready to move on to binding the API...
    //
    // Here is where you'd define your API definition for the native Objective-C library.
    //
    // For example, to bind the following Objective-C class:
    //
    //     @interface Widget : NSObject {
    //     }
    //
    // The C# binding would look like this:
    //
    //     [BaseType (typeof (NSObject))]
    //     interface Widget {
    //     }
    //
    // To bind Objective-C properties, such as:
    //
    //     @property (nonatomic, readwrite, assign) CGPoint center;
    //
    // You would add a property definition in the C# interface like so:
    //
    //     [Export ("center")]
    //     CGPoint Center { get; set; }
    //
    // To bind an Objective-C method, such as:
    //
    //     -(void) doSomething:(NSObject *)object atIndex:(NSInteger)index;
    //
    // You would add a method definition to the C# interface like so:
    //
    //     [Export ("doSomething:atIndex:")]
    //     void DoSomething (NSObject object, nint index);
    //
    // Objective-C "constructors" such as:
    //
    //     -(id)initWithElmo:(ElmoMuppet *)elmo;
    //
    // Can be bound as:
    //
    //     [Export ("initWithElmo:")]
    //     NativeHandle Constructor (ElmoMuppet elmo);
    //
    // For more information, see https://aka.ms/ios-binding
    //
    
    // @protocol OnInitializedListener
    [Protocol]
    [Model]
    [BaseType(typeof(NSObject))]
    interface OnInitializedListener
    {
        [Abstract]
        [Export("onInitialized")]
        void OnInitialized();

        [Abstract]
        [Export("onInitializationFailedWithError:")]
        void OnInitializationFailed(string error);
    }

    // @protocol OnAdLoadedListener
    [Protocol]
    [Model]
    [BaseType(typeof(NSObject))]
    interface OnAdLoadedListener
    {
        [Abstract]
        [Export("onAdLoaded")]
        void OnAdLoaded();

        [Abstract]
        [Export("onAdFailedToLoadWithErrorCode:errorMessage:")]
        void OnAdFailedToLoad(NSObject errorCode, string errorMessage);
    }

    // @protocol OnAdEventListener
    [Protocol]
    [Model]
    [BaseType(typeof(NSObject))]
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
        void OnAdFailedToShow(NSObject errorCode, string errorMessage);
    }

    // @protocol OnRewardEarnedListener
    [Protocol]
    [Model]
    [BaseType(typeof(NSObject))]
    interface OnRewardEarnedListener
    {
        [Abstract]
        [Export("onRewardEarnedWithType:amount:")]
        void OnRewardEarned(string type, NSObject amount);
    }

    // @interface AdMobManager
    [DisableDefaultCtor]
    [BaseType(typeof(NSObject))]
    interface AdMobManager
    {
        [Static]
        [Export("instance", ArgumentSemantic.Strong)]
        AdMobManager Instance { get; }

        [Export("initializeWithViewController:listener:")]
        void Initialize(UIViewController viewController, IOnInitializedListener listener);

        [Export("isInitialized")]
        bool IsInitialized();
    }

    // @interface BannerAdWrapper
    [BaseType(typeof(NSObject))]
    interface BannerAdWrapper
    {
        [Export("loadWithAdUnitId:viewController:loadListener:eventListener:")]
        UIView Load(string adUnitId,
                    UIViewController viewController,
                    IOnAdLoadedListener loadListener,
                    [NullAllowed] IOnAdEventListener eventListener);

        [Export("destroy")]
        void Destroy();
    }

    // @interface InterstitialAdWrapper
    [BaseType(typeof(NSObject))]
    interface InterstitialAdWrapper
    {
        [Export("loadWithAdUnitId:loadListener:eventListener:")]
        void Load(string adUnitId,
                  IOnAdLoadedListener loadListener,
                  [NullAllowed] IOnAdEventListener eventListener);

        [Export("showWithViewController:")]
        void Show(UIViewController viewController);

        [Export("isLoaded")]
        bool IsLoaded();
    }

    // @interface RewardedAdWrapper
    [BaseType(typeof(NSObject))]
    interface RewardedAdWrapper
    {
        [Export("loadWithAdUnitId:loadListener:eventListener:")]
        void Load(string adUnitId,
                  IOnAdLoadedListener loadListener,
                  [NullAllowed] IOnAdEventListener eventListener);

        [Export("showWithViewController:rewardListener:")]
        void Show(UIViewController viewController, IOnRewardEarnedListener rewardListener);

        [Export("isLoaded")]
        bool IsLoaded();
    }

    // @interface AppOpenAdWrapper
    [BaseType(typeof(NSObject))]
    interface AppOpenAdWrapper
    {
        [Export("loadWithAdUnitId:loadListener:eventListener:")]
        void Load(string adUnitId,
                  IOnAdLoadedListener loadListener,
                  [NullAllowed] IOnAdEventListener eventListener);

        [Export("showWithViewController:")]
        void Show(UIViewController viewController);

        [Export("isLoaded")]
        bool IsLoaded();

        [Export("isShowing")]
        bool IsShowing();
    }
}


