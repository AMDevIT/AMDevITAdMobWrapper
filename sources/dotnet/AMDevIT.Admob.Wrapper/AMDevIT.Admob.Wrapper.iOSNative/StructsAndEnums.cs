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

    public partial interface IOnInitializedListener
    {

    }

    public partial interface IOnAdEventListener
    {
        
    }

    public partial interface IOnRewardEarnedListener
    {

    }

    public partial interface IOnAdLoadedListener
    {
        
    }
}
