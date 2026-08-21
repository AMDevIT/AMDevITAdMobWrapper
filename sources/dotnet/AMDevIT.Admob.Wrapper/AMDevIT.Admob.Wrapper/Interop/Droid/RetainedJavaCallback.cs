#if ANDROID

using Android.Runtime;
using System.Collections.Concurrent;

namespace AMDevIT.Admob.Wrapper.Interop.Droid;

internal abstract class RetainedJavaCallback : Java.Lang.Object
{
    #region Fields

    private static readonly ConcurrentDictionary<long, RetainedJavaCallback> callbacks = new();
    private static long nextCallbackId;
    private readonly long callbackId;

    #endregion

    #region .ctor

    protected RetainedJavaCallback()
    {
        this.callbackId = Interlocked.Increment(ref nextCallbackId);
        callbacks.TryAdd(this.callbackId, this);
    }

    protected RetainedJavaCallback(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    #endregion

    #region Methods

    protected void Release()
    {
        if (this.callbackId != 0)
            callbacks.TryRemove(this.callbackId, out RetainedJavaCallback? _);
    }

    #endregion
}

#endif
