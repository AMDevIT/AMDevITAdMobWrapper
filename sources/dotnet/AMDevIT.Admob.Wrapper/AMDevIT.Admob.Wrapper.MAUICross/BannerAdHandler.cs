using Microsoft.Maui.Handlers;

namespace AMDevIT.Admob.Wrapper.MAUICross
{
    public partial class BannerAdHandler
#if ANDROID
        : ViewHandler<BannerAd, Android.Views.View>
#elif IOS
        : ViewHandler<BannerAd, UIKit.UIView>
#elif MACCATALYST
        : ViewHandler<BannerAd, UIKit.UIView>
#elif WINDOWS
        : ViewHandler<BannerAd, Microsoft.UI.Xaml.FrameworkElement>
#else
        : ViewHandler<BannerAd, object>
#endif
    {
        #region Fields

        private static PropertyMapper<BannerAd, BannerAdHandler> mapper = new(ViewHandler.ViewMapper)
        {
            [nameof(BannerAd.AdUnitId)] = MapAdUnitId,
            [nameof(BannerAd.AdSize)] = MapAdSize,
            [nameof(BannerAd.FallbackTemplate)] = MapFallbackTemplate,
        };

        #endregion

        #region Properties

        public static PropertyMapper<BannerAd, BannerAdHandler> Mapper 
        { 
            get => mapper; 
            set => mapper = value; 
        }

        #endregion

        #region .ctor

        public BannerAdHandler() 
            : base(Mapper) 
        { 
        }

        #endregion

        #region Methods

        private static void MapAdUnitId(BannerAdHandler handler, BannerAd view)
        {
            handler.UpdateAdUnitId();
        }

        private static void MapAdSize(BannerAdHandler handler, BannerAd view)
        {
            handler.UpdateAdSize();
        }

        private static void MapFallbackTemplate(BannerAdHandler handler, BannerAd view)
        {
            handler.UpdateFallbackTemplate();
        }

        partial void UpdateAdUnitId();
        partial void UpdateAdSize();
        partial void UpdateFallbackTemplate();

        #endregion
    }
}
