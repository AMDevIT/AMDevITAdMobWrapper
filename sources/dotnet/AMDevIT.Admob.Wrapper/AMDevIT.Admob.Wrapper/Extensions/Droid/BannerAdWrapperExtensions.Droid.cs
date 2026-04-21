#if ANDROID

using AMDevIT.Admob.Wrapper.Ads;
using AMDevIT.Admob.Wrapper.Listeners;
using Android.Views;

namespace AMDevIT.Admob.Wrapper.Extensions.Droid;

public static class BannerAdWrapperExtensions
{
    #region Methods

    public static Task<View> LoadAsync(this BannerAdWrapper wrapper,
                                       string adUnitId,
                                       BannerAdViewSize? adSize = null)
    {
        TaskCompletionSource<View> tcs = new ();
        View? adView = null;

        LoadListener loadListener = new (onLoaded: () => tcs.SetResult(adView!),
                                         onFailed: (code, msg) => tcs.SetException(new AdException(code, msg)));

        adView = adSize != null ? wrapper.Load(adUnitId, adSize, loadListener) : wrapper.Load(adUnitId, loadListener);
        return tcs.Task;
    }

    #endregion

    #region Nested listener classes

    private class LoadListener(Action onLoaded, Action<int, string> onFailed) 
        : Java.Lang.Object, IOnAdLoadedListener
    {
        private readonly Action onLoaded = onLoaded;
        private readonly Action<int, string> onFailed = onFailed;

        public void OnAdLoaded() => this.onLoaded();

        public void OnAdFailedToLoad(int errorCode, string errorMessage) =>
            this.onFailed(errorCode, errorMessage);
    }

    #endregion
}

#endif