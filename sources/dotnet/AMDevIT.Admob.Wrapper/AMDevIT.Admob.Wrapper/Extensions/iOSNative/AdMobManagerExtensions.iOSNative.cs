#if IOS

using AMDevIT.Admob.Wrapper.iOSNative;

namespace AMDevIT.Admob.Wrapper.Extensions.iOSNative;

public static class AdMobManagerExtensions
{
    public static Task InitializeAsync(this AdMobManager manager, UIViewController viewController)
    {
        TaskCompletionSource tcs = new ();
        manager.InitializeWithViewController(viewController, new InitListener(tcs));
        return tcs.Task;
    }


    #region Nested classes

    private class InitListener(TaskCompletionSource tcs) 
        : NSObject, IOnInitializedListener
    {
        #region Fields

        private readonly TaskCompletionSource tcs = tcs;

        #endregion

        #region Methods

        public void OnInitialized() => this.tcs.SetResult();
        public void OnInitializationFailedWithError(string error) =>
            this.tcs.SetException(new AdException(-1, error));

        #endregion
    }

    #endregion
}

#endif
