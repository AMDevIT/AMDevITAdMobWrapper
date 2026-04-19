#if ANDROID

using AMDevIT.Admob.Wrapper.Listeners;
using Android.Content;

namespace AMDevIT.Admob.Wrapper.Extensions.Droid;

public static partial class AdMobManagerExtensions
{
    #region Methods

    public static Task InitializeAsync(this AdMobManager manager, Context context)
    {
        TaskCompletionSource tcs = new ();
        manager.Initialize(context, new InitListener(tcs));
        return tcs.Task;
    }

    #endregion

    #region Nested listener classes

    private class InitListener(TaskCompletionSource tcs) 
        : Java.Lang.Object, IOnInitializedListener
    {
        #region Fields

        private readonly TaskCompletionSource tcs = tcs;

        #endregion

        #region Methods

        public void OnInitialized() => this.tcs.SetResult();

        public void OnInitializationFailed(string error) => this.tcs.SetException(new Exception(error));

        #endregion
    }

    #endregion
}

#endif 
