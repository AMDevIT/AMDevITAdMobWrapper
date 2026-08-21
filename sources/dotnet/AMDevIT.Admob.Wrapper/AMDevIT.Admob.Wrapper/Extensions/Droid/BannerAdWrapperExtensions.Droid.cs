#if ANDROID

using AMDevIT.Admob.Wrapper.Ads;
using AMDevIT.Admob.Wrapper.Interop.Droid;
using AMDevIT.Admob.Wrapper.Listeners;
using Android.Runtime;
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

    private class LoadListener : RetainedJavaCallback, IOnAdLoadedListener
    {
        #region Fields

        private readonly Action? onLoaded;
        private readonly Action<int, string>? onFailed;

        #endregion

        #region .ctor

        public LoadListener()
        {
        }

        public LoadListener(Action onLoaded, Action<int, string> onFailed)
        {
            this.onLoaded = onLoaded;
            this.onFailed = onFailed;
        }

        protected LoadListener(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        #endregion

        #region Methods

        public void OnAdLoaded()
        {
            try
            {
                this.onLoaded?.Invoke();
            }
            finally
            {
                this.Release();
            }
        }

        public void OnAdFailedToLoad(int errorCode, string errorMessage)
        {
            try
            {
                this.onFailed?.Invoke(errorCode, errorMessage);
            }
            finally
            {
                this.Release();
            }
        }

        #endregion
    }

    #endregion
}

#endif
