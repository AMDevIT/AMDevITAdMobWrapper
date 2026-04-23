using AMDevIT.Admob.Wrapper.MAUICross.Services;

namespace AMDevIT.Admob.Wrapper.MAUICross;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder UseAMDevITAdMobWrapper(this MauiAppBuilder builder)
    {
        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddHandler<BannerAd, BannerAdHandler>();
        });

        builder.Services.AddSingleton<IInterstitialAdService, InterstitialAdService>();
        builder.Services.AddSingleton<IAppOpenAdService, AppOpenAdService>();
        builder.Services.AddSingleton<IShowableRewardedAdService, RewardedAdService>();

        return builder;
    }
}
