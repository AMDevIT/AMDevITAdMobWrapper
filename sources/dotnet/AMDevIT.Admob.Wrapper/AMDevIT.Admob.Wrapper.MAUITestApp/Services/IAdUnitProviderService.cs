namespace AMDevIT.Admob.Wrapper.MAUITestApp.Services
{
    public interface IAdUnitProviderService
    {
        string GetAppOpenAdUnitId();
        string GetBannerAdUnitId();
        string GetInterstitialAdUnitId();
        string GetRewardedAdUnitId();
    }
}