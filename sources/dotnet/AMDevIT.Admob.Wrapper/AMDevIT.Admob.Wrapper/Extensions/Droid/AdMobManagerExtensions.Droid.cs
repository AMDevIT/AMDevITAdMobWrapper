#if ANDROID

using AMDevIT.Admob.Wrapper.Listeners;
using AMDevIT.Admob.Wrapper.Interop.Droid;
using Android.App;
using Android.Content;
using Android.Runtime;
using ManagedAdMobAgeTreatment = AMDevIT.Admob.Wrapper.AdMobAgeTreatment;
using NativeAdMobAgeTreatment = AMDevIT.Admob.Wrapper.Privacy.AdMobAgeTreatment;
using NativeConsentDebugParameters = AMDevIT.Admob.Wrapper.Privacy.ConsentInformationRequestDebugParameters;
using NativeConsentStatusData = AMDevIT.Admob.Wrapper.Privacy.ConsentStatusData;

namespace AMDevIT.Admob.Wrapper.Extensions.Droid;

public static partial class AdMobManagerExtensions
{
    #region Methods

    public static Task InitializeAsync(this AdMobManager manager,
                                       Context context,
                                       string applicationId,
                                       CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrWhiteSpace(applicationId);
        cancellationToken.ThrowIfCancellationRequested();

        TaskCompletionSource completionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);
        InitListener listener = new(completionSource, cancellationToken);
        manager.Initialize(context, applicationId, listener);
        return completionSource.Task;
    }

    public static Task InitializeAsync(this AdMobManager manager,
                                       Context context,
                                       string applicationId,
                                       ManagedAdMobAgeTreatment ageTreatment,
                                       CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrWhiteSpace(applicationId);
        cancellationToken.ThrowIfCancellationRequested();

        NativeAdMobAgeTreatment nativeAgeTreatment = ageTreatment switch
        {
            ManagedAdMobAgeTreatment.Unspecified => NativeAdMobAgeTreatment.Unspecified,
            ManagedAdMobAgeTreatment.Child => NativeAdMobAgeTreatment.Child,
            ManagedAdMobAgeTreatment.Teen => NativeAdMobAgeTreatment.Teen,
            _ => throw new ArgumentOutOfRangeException(nameof(ageTreatment), ageTreatment, null)
        };
        TaskCompletionSource completionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);
        InitListener listener = new(completionSource, cancellationToken);
        manager.Initialize(context, applicationId, nativeAgeTreatment, listener);
        return completionSource.Task;
    }

    public static Task<ConsentInformationSnapshot> UpdateCurrentConsentInformationAsync(
        this AdMobManager manager,
        Activity activity,
        ConsentRequestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(activity);
        cancellationToken.ThrowIfCancellationRequested();

        options ??= new ConsentRequestOptions();
        TaskCompletionSource<ConsentInformationSnapshot> completionSource =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        ConsentInformationRequestListener listener = new(manager, completionSource, cancellationToken);
        NativeConsentDebugParameters? debugParameters = CreateNativeDebugParameters(options.DebugParameters);

        manager.UpdateCurrentConsentInformation(activity,
                                                options.TagForUnderAgeOfConsent,
                                                listener,
                                                debugParameters);
        return completionSource.Task;
    }

    public static Task<ConsentGatheringResult> GatherConsentAsync(
        this AdMobManager manager,
        Activity activity,
        ConsentRequestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(activity);
        cancellationToken.ThrowIfCancellationRequested();

        options ??= new ConsentRequestOptions();
        TaskCompletionSource<ConsentGatheringResult> completionSource =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        ConsentGatheringListener listener = new(completionSource, cancellationToken);
        NativeConsentDebugParameters? debugParameters = CreateNativeDebugParameters(options.DebugParameters);

        manager.GatherConsent(activity,
                              options.TagForUnderAgeOfConsent,
                              listener,
                              debugParameters);
        return completionSource.Task;
    }

    public static Task ShowPrivacyOptionsFormAsync(this AdMobManager manager,
                                                   Activity activity,
                                                   CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(activity);
        cancellationToken.ThrowIfCancellationRequested();

        TaskCompletionSource completionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);
        ConsentFormListener listener = new(completionSource, cancellationToken);
        manager.ShowPrivacyOptionsForm(activity, listener);
        return completionSource.Task;
    }

    public static Task LoadAndShowConsentFormIfRequiredAsync(
        this AdMobManager manager,
        Activity activity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(activity);
        cancellationToken.ThrowIfCancellationRequested();

        TaskCompletionSource completionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);
        ConsentFormListener listener = new(completionSource, cancellationToken);
        manager.LoadAndShowConsentFormIfRequired(activity, listener);
        return completionSource.Task;
    }

    public static ConsentInformationSnapshot? GetCurrentConsentInformation(this AdMobManager manager)
    {
        ArgumentNullException.ThrowIfNull(manager);
        NativeConsentStatusData? consentInformation = manager.CurrentConsentInformation();
        return consentInformation == null ? null : MapConsentInformation(consentInformation);
    }

    private static NativeConsentDebugParameters? CreateNativeDebugParameters(ConsentDebugParameters? parameters)
    {
        if (parameters == null)
            return null;

        Java.Lang.Integer? debugGeography = parameters.DebugGeography.HasValue
            ? Java.Lang.Integer.ValueOf((int)parameters.DebugGeography.Value)
            : null;
        return new NativeConsentDebugParameters(debugGeography, parameters.TestDeviceHashedId);
    }

    private static ConsentInformationSnapshot MapConsentInformation(NativeConsentStatusData information)
    {
        DateTimeOffset? lastRefresh = information.LastRefreshTimestampMilliseconds > 0
            ? DateTimeOffset.FromUnixTimeMilliseconds(information.LastRefreshTimestampMilliseconds)
            : null;

        return new ConsentInformationSnapshot(
            lastRefresh,
            (ConsentStatus)information.ConsentStatus,
            (PrivacyOptionsRequirementStatus)information.PrivacyOptionsRequirementStatus);
    }

    #endregion

    private class InitListener : RetainedJavaCallback, IOnInitializedListener
    {
        #region Fields

        private readonly TaskCompletionSource? completionSource;
        private readonly CancellationTokenRegistration cancellationRegistration;

        #endregion

        #region .ctor

        public InitListener()
        {
        }

        public InitListener(TaskCompletionSource completionSource, CancellationToken cancellationToken)
        {
            this.completionSource = completionSource;
            this.cancellationRegistration = cancellationToken.Register(() =>
                this.completionSource?.TrySetCanceled(cancellationToken));
        }

        protected InitListener(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        #endregion

        #region Methods

        public void OnInitialized()
        {
            try
            {
                this.cancellationRegistration.Dispose();
                this.completionSource?.TrySetResult();
            }
            finally
            {
                this.Release();
            }
        }

        public void OnInitializationFailed(string error)
        {
            try
            {
                this.cancellationRegistration.Dispose();
                this.completionSource?.TrySetException(new InvalidOperationException(error));
            }
            finally
            {
                this.Release();
            }
        }

        #endregion
    }

    private class ConsentInformationRequestListener
        : RetainedJavaCallback, IOnConsentInformationRequestListener
    {
        #region Fields

        private readonly AdMobManager? manager;
        private readonly TaskCompletionSource<ConsentInformationSnapshot>? completionSource;
        private readonly CancellationTokenRegistration cancellationRegistration;

        #endregion

        #region .ctor

        public ConsentInformationRequestListener()
        {
        }

        public ConsentInformationRequestListener(
            AdMobManager manager,
            TaskCompletionSource<ConsentInformationSnapshot> completionSource,
            CancellationToken cancellationToken)
        {
            this.manager = manager;
            this.completionSource = completionSource;
            this.cancellationRegistration = cancellationToken.Register(() =>
                this.completionSource?.TrySetCanceled(cancellationToken));
        }

        protected ConsentInformationRequestListener(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        #endregion

        #region Methods

        public void OnConsentInformationRequestSuccess()
        {
            try
            {
                this.cancellationRegistration.Dispose();
                if (this.manager == null || this.completionSource == null)
                    return;

                ConsentInformationSnapshot? information = this.manager.GetCurrentConsentInformation();

                if (information == null)
                {
                    this.completionSource.TrySetException(
                        new InvalidOperationException("Consent information was not available after a successful update."));
                    return;
                }

                this.completionSource.TrySetResult(information);
            }
            finally
            {
                this.Release();
            }
        }

        public void OnConsentInformationRequestFailure(int errorCode, string errorMessage)
        {
            try
            {
                this.cancellationRegistration.Dispose();
                this.completionSource?.TrySetException(new ConsentException(errorCode, errorMessage));
            }
            finally
            {
                this.Release();
            }
        }

        #endregion
    }

    private class ConsentGatheringListener : RetainedJavaCallback, IOnConsentGatheringListener
    {
        #region Fields

        private readonly TaskCompletionSource<ConsentGatheringResult>? completionSource;
        private readonly CancellationTokenRegistration cancellationRegistration;

        #endregion

        #region .ctor

        public ConsentGatheringListener()
        {
        }

        public ConsentGatheringListener(TaskCompletionSource<ConsentGatheringResult> completionSource,
                                        CancellationToken cancellationToken)
        {
            this.completionSource = completionSource;
            this.cancellationRegistration = cancellationToken.Register(() =>
                this.completionSource?.TrySetCanceled(cancellationToken));
        }

        protected ConsentGatheringListener(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        #endregion

        #region Methods

        public void OnCompleted(bool canRequestAds, bool privacyOptionsRequired)
        {
            try
            {
                this.cancellationRegistration.Dispose();
                this.completionSource?.TrySetResult(
                    new ConsentGatheringResult(canRequestAds, privacyOptionsRequired));
            }
            finally
            {
                this.Release();
            }
        }

        public void OnCompletedWithError(int errorCode,
                                         string errorMessage,
                                         bool canRequestAds,
                                         bool privacyOptionsRequired)
        {
            try
            {
                this.cancellationRegistration.Dispose();
                this.completionSource?.TrySetException(
                    new ConsentException(errorCode,
                                         errorMessage,
                                         canRequestAds,
                                         privacyOptionsRequired));
            }
            finally
            {
                this.Release();
            }
        }

        #endregion
    }

    private class ConsentFormListener : RetainedJavaCallback, IOnConsentFormEventListener
    {
        #region Fields

        private readonly TaskCompletionSource? completionSource;
        private readonly CancellationTokenRegistration cancellationRegistration;

        #endregion

        #region .ctor

        public ConsentFormListener()
        {
        }

        public ConsentFormListener(TaskCompletionSource completionSource,
                                   CancellationToken cancellationToken)
        {
            this.completionSource = completionSource;
            this.cancellationRegistration = cancellationToken.Register(() =>
                this.completionSource?.TrySetCanceled(cancellationToken));
        }

        protected ConsentFormListener(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        #endregion

        #region Methods

        public void OnDismissed()
        {
            try
            {
                this.cancellationRegistration.Dispose();
                this.completionSource?.TrySetResult();
            }
            finally
            {
                this.Release();
            }
        }

        public void OnDismissedWithError(int errorCode, string? errorMessage)
        {
            try
            {
                this.cancellationRegistration.Dispose();
                this.completionSource?.TrySetException(new ConsentException(errorCode, errorMessage));
            }
            finally
            {
                this.Release();
            }
        }

        #endregion
    }
}

#endif
