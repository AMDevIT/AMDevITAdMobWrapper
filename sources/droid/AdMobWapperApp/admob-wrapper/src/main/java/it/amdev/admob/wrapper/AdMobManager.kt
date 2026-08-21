package it.amdev.admob.wrapper

import android.app.Activity
import android.content.Context
import com.google.android.libraries.ads.mobile.sdk.MobileAds
import com.google.android.libraries.ads.mobile.sdk.common.AgeRestrictedTreatment
import com.google.android.libraries.ads.mobile.sdk.common.RequestConfiguration
import com.google.android.libraries.ads.mobile.sdk.initialization.InitializationConfig
import com.google.android.ump.ConsentDebugSettings
import com.google.android.ump.ConsentInformation
import com.google.android.ump.ConsentRequestParameters
import com.google.android.ump.UserMessagingPlatform
import it.amdev.admob.wrapper.diagnostics.IDroidLogger
import it.amdev.admob.wrapper.listeners.OnConsentFormEventListener
import it.amdev.admob.wrapper.listeners.OnConsentGatheringListener
import it.amdev.admob.wrapper.listeners.OnConsentInformationRequestListener
import it.amdev.admob.wrapper.listeners.OnInitializedListener
import it.amdev.admob.wrapper.privacy.AdMobAgeTreatment
import it.amdev.admob.wrapper.privacy.ConsentInformationRequestDebugParameters
import it.amdev.admob.wrapper.privacy.ConsentStatusData
import java.time.Instant

@Suppress("unused")
class AdMobManager(private val logger: IDroidLogger? = null) {

    private var initialized = false
    private var consentInformation: ConsentInformation? = null

    @Volatile
    private var lastConsentRefreshTimestampMilliseconds: Long? = null

    companion object {
        private const val LOG_TAG = "AdMobManager"

        @JvmStatic
        val instance: AdMobManager by lazy { AdMobManager() }
    }

    fun initialize(context: Context,
                   applicationId: String,
                   listener: OnInitializedListener) {
        val initializationConfig = InitializationConfig.Builder(applicationId = applicationId)
                                                       .build()
        initialize(context = context,
                   initializationConfig = initializationConfig,
                   listener = listener)
    }

    fun initialize(context: Context,
                   applicationId: String,
                   ageTreatment: AdMobAgeTreatment,
                   listener: OnInitializedListener) {
        val requestConfiguration = RequestConfiguration.Builder()
                                                       .setAgeRestrictedTreatment(
                                                           ageTreatment.toNativeAgeRestrictedTreatment()
                                                       )
                                                       .build()
        val initializationConfig = InitializationConfig.Builder(applicationId = applicationId)
                                                       .setRequestConfiguration(requestConfiguration)
                                                       .build()

        if (initialized)
            MobileAds.setRequestConfiguration(requestConfiguration)

        initialize(context = context,
                   initializationConfig = initializationConfig,
                   listener = listener)
    }

    fun isInitialized(): Boolean = initialized

    fun updateCurrentConsentInformation(activity: Activity,
                                        tagForUnderAgeOfConsent: Boolean,
                                        listener: OnConsentInformationRequestListener,
                                        requestDebugParameters: ConsentInformationRequestDebugParameters? = null) {
        val consentInformation = this.consentInformation ?: UserMessagingPlatform
                                                            .getConsentInformation(activity.applicationContext)
                                                            .also { this.consentInformation = it }

        var parametersBuilder = ConsentRequestParameters.Builder().setTagForUnderAgeOfConsent(tagForUnderAgeOfConsent)

        requestDebugParameters?.let { debugParameters ->
            val debugSettingsBuilder =
                ConsentDebugSettings.Builder(activity)

            debugParameters.debugGeography?.let {
                debugSettingsBuilder.setDebugGeography(it)
            }

            debugParameters.testDeviceHashedId
                ?.takeIf(String::isNotBlank)
                ?.let(debugSettingsBuilder::addTestDeviceHashedId)

            parametersBuilder = parametersBuilder.setConsentDebugSettings(
                    debugSettingsBuilder.build()
                )
        }

        val parameters = parametersBuilder.build()

        consentInformation.requestConsentInfoUpdate(activity,
                                                parameters,
            {
                lastConsentRefreshTimestampMilliseconds = Instant.now().toEpochMilli()
                listener.onConsentInformationRequestSuccess()
            },
            { error ->
                listener.onConsentInformationRequestFailure(
                    errorCode = error.errorCode,
                    errorMessage = error.message
                )
            }
        )
    }

    fun currentConsentInformation()
        : ConsentStatusData? {
        if (this.consentInformation == null)
            return null

        val currentConsentStatus = this.consentInformation?.consentStatus ?: ConsentInformation.ConsentStatus.UNKNOWN
        val currentPrivacyOptionRequirements = this.consentInformation?.privacyOptionsRequirementStatus ?: ConsentInformation.PrivacyOptionsRequirementStatus.UNKNOWN

        val consentStatusValue: Int = currentConsentStatus

        val privacyOptionRequirementsValue = when(currentPrivacyOptionRequirements) {
            ConsentInformation.PrivacyOptionsRequirementStatus.UNKNOWN -> 0
            ConsentInformation.PrivacyOptionsRequirementStatus.REQUIRED -> 1
            ConsentInformation.PrivacyOptionsRequirementStatus.NOT_REQUIRED -> 2
        }

        val epochTimestamp = lastConsentRefreshTimestampMilliseconds ?: 0L
        val consentStatusData = ConsentStatusData(lastRefreshTimestampMilliseconds = epochTimestamp,
                                                  consentStatus = consentStatusValue,
                                                  privacyOptionsRequirementStatus = privacyOptionRequirementsValue)

        return consentStatusData
    }

    fun showPrivacyOptionsForm(activity: Activity,
                               listener: OnConsentFormEventListener)
    {

        val information = consentInformation

        if (information == null) {
            listener.onDismissedWithError(errorCode = -1,
                                          errorMessage =
                                          "Consent information has not been updated.")
            return
        }

        if (information.privacyOptionsRequirementStatus !=
            ConsentInformation.PrivacyOptionsRequirementStatus.REQUIRED)
        {
            listener.onDismissedWithError(errorCode = -2,
                                          errorMessage =
                                          "Privacy options form is not required.")
            return
        }

        UserMessagingPlatform.showPrivacyOptionsForm(activity) { formError ->
            if (formError != null) {
                logger?.logError(tag = LOG_TAG,
                                 message = "Error showing privacy options form: ${formError.message}")
                listener.onDismissedWithError(errorCode = formError.errorCode,
                                              errorMessage = formError.message)
            } else {
                logger?.logDebug(tag = LOG_TAG,
                                 message = "Privacy options form dismissed successfully")
                listener.onDismissed()
            }
        }
    }

    fun loadAndShowConsentFormIfRequired(activity: Activity,
                                         listener: OnConsentFormEventListener) {
        UserMessagingPlatform.loadAndShowConsentFormIfRequired(activity)
        { formError ->
            if (formError != null) {
                logger?.logError(tag = LOG_TAG,
                                 message = "Error loading or showing the required consent form: " +
                                           formError.message)
                listener.onDismissedWithError(errorCode = formError.errorCode,
                                              errorMessage = formError.message)
            } else {
                logger?.logDebug(tag = LOG_TAG,
                    message = "Required consent form dismissed successfully")
                listener.onDismissed()
            }
        }
    }

    fun canRequestAds(): Boolean {
        val result = consentInformation?.canRequestAds() ?: false
        return result
    }

    fun gatherConsent(activity: Activity,
                      tagForUnderAgeOfConsent: Boolean,
                      listener: OnConsentGatheringListener,
                      requestDebugParameters: ConsentInformationRequestDebugParameters? = null) {

        updateCurrentConsentInformation(activity = activity,
                                        tagForUnderAgeOfConsent = tagForUnderAgeOfConsent,
                                        requestDebugParameters = requestDebugParameters,
                                        listener = object : OnConsentInformationRequestListener {

                    override fun onConsentInformationRequestSuccess() {
                        UserMessagingPlatform
                            .loadAndShowConsentFormIfRequired(activity) { error ->
                                if (error != null) {
                                    listener.onCompletedWithError(errorCode = error.errorCode,
                                                                  errorMessage = error.message,
                                                                  canRequestAds = consentInformation?.canRequestAds() ?: false,
                                                                  privacyOptionsRequired = consentInformation?.privacyOptionsRequirementStatus == ConsentInformation
                                                                                           .PrivacyOptionsRequirementStatus
                                                                                           .REQUIRED)
                                } else {
                                    listener.onCompleted(canRequestAds = consentInformation?.canRequestAds() ?: false,
                                                         privacyOptionsRequired = consentInformation?.privacyOptionsRequirementStatus == ConsentInformation
                                                                                  .PrivacyOptionsRequirementStatus
                                                                                  .REQUIRED)
                                }
                            }
                    }

                    override fun onConsentInformationRequestFailure(errorCode: Int,
                                                                    errorMessage: String) {
                        listener.onCompletedWithError(errorCode = errorCode,
                                                      errorMessage = errorMessage,
                                                      canRequestAds = consentInformation?.canRequestAds() ?: false,
                                                      privacyOptionsRequired = consentInformation?.privacyOptionsRequirementStatus == ConsentInformation
                                                                               .PrivacyOptionsRequirementStatus
                                                                               .REQUIRED)
                    }
                }
        )
    }

    fun resetConsentForTesting() {
        consentInformation?.reset()
        lastConsentRefreshTimestampMilliseconds = null
    }

    private fun initialize(context: Context,
                           initializationConfig: InitializationConfig,
                           listener: OnInitializedListener) {
        if (initialized) {
            listener.onInitialized()
            return
        }
        Thread {
            try {
                MobileAds.initialize(context, initializationConfig) {
                    initialized = true
                    listener.onInitialized()
                }
            } catch (e: Exception) {
                listener.onInitializationFailed(e.message ?: "Unknown error")
            }
        }.start()
    }

    private fun AdMobAgeTreatment.toNativeAgeRestrictedTreatment(): AgeRestrictedTreatment =
        when (this) {
            AdMobAgeTreatment.Unspecified -> AgeRestrictedTreatment.UNSPECIFIED
            AdMobAgeTreatment.Child -> AgeRestrictedTreatment.CHILD
            AdMobAgeTreatment.Teen -> AgeRestrictedTreatment.TEEN
        }
}
