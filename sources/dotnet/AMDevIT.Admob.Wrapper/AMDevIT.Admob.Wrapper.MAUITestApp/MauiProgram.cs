using AMDevIT.Admob.Wrapper.MAUICross;
using AMDevIT.Admob.Wrapper.MAUICross.Services;
using AMDevIT.Admob.Wrapper.MAUITestApp.Services;
using AMDevIT.Admob.Wrapper.MAUITestApp.ViewModels;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace AMDevIT.Admob.Wrapper.MAUITestApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>()
               .UseMauiCommunityToolkit()
               .ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });

#if DEBUG
		builder.Logging.AddDebug();
#endif
        builder.UseAMDevITAdMobWrapper();

        builder.Services.AddSingleton<IDispatcherService, DispatcherService>();
        builder.Services.AddSingleton<IAdUnitProviderService, AdUnitProviderService>();
        builder.Services.AddTransientWithShellRoute<MainPage, MainPageViewModel>("main");
        builder.Services.AddTransient<IInterstitialAdService, InterstitialAdService>();

        return builder.Build();
    }
}
