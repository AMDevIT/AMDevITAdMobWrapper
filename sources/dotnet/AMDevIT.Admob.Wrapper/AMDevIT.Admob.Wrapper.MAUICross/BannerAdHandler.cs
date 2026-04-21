using Microsoft.Maui.Handlers;

namespace AMDevIT.Admob.Wrapper.MAUICross
{
    public partial class BannerAdHandler
#if ANDROID
        : ViewHandler<BannerAd,Android.Views.View>
#elif IOS
        : ViewHandler<BannerAd, UIKit.UIView>
#else
        : ViewHandler<BannerAd, global::Microsoft.Maui.Platform.MauiView>
#endif
    {
        private static PropertyMapper<BannerAd, BannerAdHandler> mapper = new(ViewHandler.ViewMapper)
        {
            [nameof(BannerAd.AdUnitId)] = MapAdUnitId,
            [nameof(BannerAd.AdSize)] = MapAdSize,
        };

        public static PropertyMapper<BannerAd, BannerAdHandler> Mapper 
        { 
            get => mapper; 
            set => mapper = value; 
        }

        public BannerAdHandler() 
            : base(Mapper) 
        { 
        }

        private static void MapAdUnitId(BannerAdHandler handler, BannerAd view)
        {
            handler.UpdateAdUnitId();
        }

        private static void MapAdSize(BannerAdHandler handler, BannerAd view)
        {
            handler.UpdateAdSize();
        }

        partial void UpdateAdUnitId();
        partial void UpdateAdSize();
    }
}
