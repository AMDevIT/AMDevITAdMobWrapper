#if ANDROID

using Android.Content;

#endif 

#if IOS

using UIKit;

#endif

namespace AMDevIT.Admob.Wrapper.MAUICross.Services
{
    public interface IContextResolverService
    {
        #region Methods

        object? GetPlatformContext();

#if ANDROID
        Context? GetContext();
#endif

#if IOS
        UIViewController? GetViewController();
#endif 

        #endregion
    }
}
