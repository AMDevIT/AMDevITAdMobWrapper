#if IOS

using AMDevIT.Admob.Wrapper.iOSNative;
using Foundation;
using ManagedAdMobAgeTreatment = AMDevIT.Admob.Wrapper.AdMobAgeTreatment;
using NativeAdMobAgeTreatment = AMDevIT.Admob.Wrapper.iOSNative.AdMobAgeTreatment;
using NativeConsentDebugParameters = AMDevIT.Admob.Wrapper.iOSNative.ConsentInformationRequestDebugParameters;
using NativeConsentStatusData = AMDevIT.Admob.Wrapper.iOSNative.ConsentStatusData;

namespace AMDevIT.Admob.Wrapper.Extensions.iOSNative;

public static class AdMobManagerExtensions
{
    #region Methods

    public static Task InitializeAsync(this AdMobManager manager,
                                       UIViewController viewController,
                                       CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(viewController);
        cancellationToken.ThrowIfCancellationRequested();

        TaskCompletionSource completionSource =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        InitListener listener = new(completionSource, cancellationToken);
        manager.InitializeWithViewController(viewController, listener);
        return completionSource.Task;
    }

    public static Task InitializeAsync(this AdMobManager manager,
                                       UIViewController viewController,
                                       ManagedAdMobAgeTreatment ageTreatment,
                                       CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(viewController);
        cancellationToken.ThrowIfCancellationRequested();

        NativeAdMobAgeTreatment nativeAgeTreatment = ageTreatment switch
        {
            ManagedAdMobAgeTreatment.Unspecified => NativeAdMobAgeTreatment.Unspecified,
            ManagedAdMobAgeTreatment.Child => NativeAdMobAgeTreatment.Child,
            ManagedAdMobAgeTreatment.Teen => NativeAdMobAgeTreatment.Teen,
            _ => throw new ArgumentOutOfRangeException(nameof(ageTreatment), ageTreatment, null)
        };
        TaskCompletionSource completionSource =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        InitListener listener = new(completionSource, cancellationToken);
        manager.InitializeWithViewController(viewController, nativeAgeTreatment, listener);
        return completionSource.Task;
    }

    public static Task<ConsentInformationSnapshot> UpdateCurrentConsentInformationAsync(
        this AdMobManager manager,
        UIViewController viewController,
        ConsentRequestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(viewController);
        cancellationToken.ThrowIfCancellationRequested();

        options ??= new ConsentRequestOptions();
        TaskCompletionSource<ConsentInformationSnapshot> completionSource =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        ConsentInformationRequestListener listener =
            new(manager, completionSource, cancellationToken);
        NativeConsentDebugParameters? debugParameters =
            CreateNativeDebugParameters(options.DebugParameters);

        manager.UpdateCurrentConsentInformationWithViewController(
            viewController,
            options.TagForUnderAgeOfConsent,
            listener,
            debugParameters);
        return completionSource.Task;
    }

    public static Task<ConsentGatheringResult> GatherConsentAsync(
        this AdMobManager manager,
        UIViewController viewController,
        ConsentRequestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(viewController);
        cancellationToken.ThrowIfCancellationRequested();

        options ??= new ConsentRequestOptions();
        TaskCompletionSource<ConsentGatheringResult> completionSource =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        ConsentGatheringListener listener = new(completionSource, cancellationToken);
        NativeConsentDebugParameters? debugParameters =
            CreateNativeDebugParameters(options.DebugParameters);

        manager.GatherConsentWithViewController(
            viewController,
            options.TagForUnderAgeOfConsent,
            listener,
            debugParameters);
        return completionSource.Task;
    }

    public static Task ShowPrivacyOptionsFormAsync(
        this AdMobManager manager,
        UIViewController viewController,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(viewController);
        cancellationToken.ThrowIfCancellationRequested();

        TaskCompletionSource completionSource =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        ConsentFormListener listener = new(completionSource, cancellationToken);
        manager.ShowPrivacyOptionsFormWithViewController(viewController, listener);
        return completionSource.Task;
    }

    public static Task LoadAndShowConsentFormIfRequiredAsync(
        this AdMobManager manager,
        UIViewController viewController,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(viewController);
        cancellationToken.ThrowIfCancellationRequested();

        TaskCompletionSource completionSource =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        ConsentFormListener listener = new(completionSource, cancellationToken);
        manager.LoadAndShowConsentFormIfRequiredWithViewController(viewController, listener);
        return completionSource.Task;
    }

    public static ConsentInformationSnapshot? GetCurrentConsentInformation(
        this AdMobManager manager)
    {
        ArgumentNullException.ThrowIfNull(manager);
        NativeConsentStatusData? consentInformation = manager.CurrentConsentInformation();
        return consentInformation == null ? null : MapConsentInformation(consentInformation);
    }

    private static NativeConsentDebugParameters? CreateNativeDebugParameters(
        ConsentDebugParameters? parameters)
    {
        if (parameters == null)
            return null;

        NSNumber? debugGeography = parameters.DebugGeography.HasValue
            ? NSNumber.FromInt32((int)parameters.DebugGeography.Value)
            : null;
        return new NativeConsentDebugParameters(debugGeography,
                                                parameters.TestDeviceHashedId);
    }

    private static ConsentInformationSnapshot MapConsentInformation(
        NativeConsentStatusData information)
    {
        DateTimeOffset? lastRefresh = information.LastRefreshTimestampMilliseconds > 0
            ? DateTimeOffset.FromUnixTimeMilliseconds(
                information.LastRefreshTimestampMilliseconds)
            : null;

        return new ConsentInformationSnapshot(
            lastRefresh,
            (ConsentStatus)information.ConsentStatus,
            (PrivacyOptionsRequirementStatus)information.PrivacyOptionsRequirementStatus);
    }

    #endregion

    private sealed class InitListener : NSObject, IOnInitializedListener
    {
        #region Fields

        private readonly TaskCompletionSource completionSource;
        private readonly CancellationTokenRegistration cancellationRegistration;

        #endregion

        #region .ctor

        public InitListener(TaskCompletionSource completionSource,
                            CancellationToken cancellationToken)
        {
            this.completionSource = completionSource;
            this.cancellationRegistration = cancellationToken.Register(() =>
                this.completionSource.TrySetCanceled(cancellationToken));
        }

        #endregion

        #region Methods

        public void OnInitialized()
        {
            this.cancellationRegistration.Dispose();
            this.completionSource.TrySetResult();
        }

        public void OnInitializationFailedWithError(string error)
        {
            this.cancellationRegistration.Dispose();
            this.completionSource.TrySetException(new InvalidOperationException(error));
        }

        #endregion
    }

    private sealed class ConsentInformationRequestListener
        : NSObject, IOnConsentInformationRequestListener
    {
        #region Fields

        private readonly AdMobManager manager;
        private readonly TaskCompletionSource<ConsentInformationSnapshot> completionSource;
        private readonly CancellationTokenRegistration cancellationRegistration;

        #endregion

        #region .ctor

        public ConsentInformationRequestListener(
            AdMobManager manager,
            TaskCompletionSource<ConsentInformationSnapshot> completionSource,
            CancellationToken cancellationToken)
        {
            this.manager = manager;
            this.completionSource = completionSource;
            this.cancellationRegistration = cancellationToken.Register(() =>
                this.completionSource.TrySetCanceled(cancellationToken));
        }

        #endregion

        #region Methods

        public void OnConsentInformationRequestSuccess()
        {
            this.cancellationRegistration.Dispose();
            ConsentInformationSnapshot? information =
                this.manager.GetCurrentConsentInformation();

            if (information == null)
            {
                this.completionSource.TrySetException(
                    new InvalidOperationException(
                        "Consent information was not available after a successful update."));
                return;
            }

            this.completionSource.TrySetResult(information);
        }

        public void OnConsentInformationRequestFailure(nint errorCode, string errorMessage)
        {
            this.cancellationRegistration.Dispose();
            this.completionSource.TrySetException(
                new ConsentException((int)errorCode, errorMessage));
        }

        #endregion
    }

    private sealed class ConsentGatheringListener
        : NSObject, IOnConsentGatheringListener
    {
        #region Fields

        private readonly TaskCompletionSource<ConsentGatheringResult> completionSource;
        private readonly CancellationTokenRegistration cancellationRegistration;

        #endregion

        #region .ctor

        public ConsentGatheringListener(
            TaskCompletionSource<ConsentGatheringResult> completionSource,
            CancellationToken cancellationToken)
        {
            this.completionSource = completionSource;
            this.cancellationRegistration = cancellationToken.Register(() =>
                this.completionSource.TrySetCanceled(cancellationToken));
        }

        #endregion

        #region Methods

        public void OnCompleted(bool canRequestAds, bool privacyOptionsRequired)
        {
            this.cancellationRegistration.Dispose();
            this.completionSource.TrySetResult(
                new ConsentGatheringResult(canRequestAds, privacyOptionsRequired));
        }

        public void OnCompletedWithError(nint errorCode,
                                         string errorMessage,
                                         bool canRequestAds,
                                         bool privacyOptionsRequired)
        {
            this.cancellationRegistration.Dispose();
            this.completionSource.TrySetException(
                new ConsentException((int)errorCode,
                                     errorMessage,
                                     canRequestAds,
                                     privacyOptionsRequired));
        }

        #endregion
    }

    private sealed class ConsentFormListener : NSObject, IOnConsentFormEventListener
    {
        #region Fields

        private readonly TaskCompletionSource completionSource;
        private readonly CancellationTokenRegistration cancellationRegistration;

        #endregion

        #region .ctor

        public ConsentFormListener(TaskCompletionSource completionSource,
                                   CancellationToken cancellationToken)
        {
            this.completionSource = completionSource;
            this.cancellationRegistration = cancellationToken.Register(() =>
                this.completionSource.TrySetCanceled(cancellationToken));
        }

        #endregion

        #region Methods

        public void OnDismissed()
        {
            this.cancellationRegistration.Dispose();
            this.completionSource.TrySetResult();
        }

        public void OnDismissedWithError(nint errorCode, string? errorMessage)
        {
            this.cancellationRegistration.Dispose();
            this.completionSource.TrySetException(
                new ConsentException((int)errorCode, errorMessage));
        }

        #endregion
    }
}

#endif
