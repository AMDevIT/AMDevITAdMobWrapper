using ObjCRuntime;

namespace AMDevIT.Admob.Wrapper.iOSNative
{
    [Native]
    public enum BannerAdViewSize : long
    {
        Adaptive,
        Banner,
        LargeBanner,
        MediumRectangle,
        FullBanner,
        Leaderboard
    }

    [Native]
    public enum AppleLogLevel : long
    {
        Trace,
        Debug,
        Information,
        Warning,
        Error,
        Critical,
        None
    }

    public partial interface IAppleLogger
    {
    }

    public partial interface IOnAdEventListener
    {
    }

    public partial interface IOnAdLoadedListener
    {
    }

    public partial interface IOnConsentFormEventListener
    {
    }

    public partial interface IOnConsentGatheringListener
    {
    }

    public partial interface IOnConsentInformationRequestListener
    {
    }

    public partial interface IOnInitializedListener
    {
    }

    public partial interface IOnRewardEarnedListener
    {
    }
}
