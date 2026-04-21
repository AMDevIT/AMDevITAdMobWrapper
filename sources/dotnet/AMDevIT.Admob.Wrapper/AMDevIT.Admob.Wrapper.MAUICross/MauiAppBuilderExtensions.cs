namespace AMDevIT.Admob.Wrapper.MAUICross;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder UseAMDevITAdMobWrapper(this MauiAppBuilder builder)
    {
        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddHandler<BannerAd, BannerAdHandler>();
        });

        return builder;
    }
}
