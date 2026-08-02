package it.amdev.admob.wrapper

import android.app.Activity
import android.content.Context
import com.google.android.libraries.ads.mobile.sdk.MobileAds
import com.google.android.libraries.ads.mobile.sdk.initialization.InitializationConfig
import com.google.android.ump.ConsentDebugSettings
import com.google.android.ump.ConsentInformation
import com.google.android.ump.ConsentRequestParameters
import com.google.android.ump.UserMessagingPlatform
import it.amdev.admob.wrapper.diagnostics.IDroidLogger
import it.amdev.admob.wrapper.listeners.OnConsentFormEventListener
import it.amdev.admob.wrapper.listeners.OnConsentInformationRequestListener
import it.amdev.admob.wrapper.listeners.OnInitializedListener
import it.amdev.admob.wrapper.privacy.ConsentInformationRequestDebugParameters
import it.amdev.admob.wrapper.privacy.ConsentStatusData
import java.time.Instant

@Suppress("unused")
class AdMobManager(private val logger: IDroidLogger? = null) {

    private var initialized = false
    private var consentInformation: ConsentInformation? = null

    companion object {
        private const val LOG_TAG = "AdMobManager"

        @JvmStatic
        val instance: AdMobManager by lazy { AdMobManager() }
    }

    fun initialize(context: Context,
                   applicationId: String,
                   listener: OnInitializedListener) {
        if (initialized) {
            listener.onInitialized()
            return
        }
        Thread {
            try {
                val initializationConfig = InitializationConfig.Builder(applicationId = applicationId)
                                                               .build()
                MobileAds.initialize(context, initializationConfig) {
                    initialized = true
                    listener.onInitialized()
                }
            } catch (e: Exception) {
                listener.onInitializationFailed(e.message ?: "Unknown error")
            }
        }.start()
    }

    fun isInitialized(): Boolean = initialized

    fun updateCurrentConsentInformation(activity: Activity,
                                        tagForUnderAgeOfConsent: Boolean,
                                        listener: OnConsentInformationRequestListener,
                                        requestDebugParameters: ConsentInformationRequestDebugParameters? = null) {
        if (this.consentInformation == null) {
            this.consentInformation = UserMessagingPlatform.getConsentInformation(activity)
        } else {
            var consentRequestParametersBuilder = ConsentRequestParameters.Builder()

            if (tagForUnderAgeOfConsent) {
                consentRequestParametersBuilder = consentRequestParametersBuilder.setTagForUnderAgeOfConsent(true)
            }

            if (requestDebugParameters != null) {
                val debugSettingsBuilder = ConsentDebugSettings.Builder(activity)

                if (requestDebugParameters.debugGeography != null) {
                    debugSettingsBuilder.setDebugGeography(requestDebugParameters.debugGeography)
                }

                if (!requestDebugParameters.testDeviceHashedId.isNullOrBlank()) {
                    debugSettingsBuilder.addTestDeviceHashedId(requestDebugParameters.testDeviceHashedId)
                }

                consentRequestParametersBuilder.setConsentDebugSettings(debugSettingsBuilder.build())
            }

            val consentRequestParameters: ConsentRequestParameters = consentRequestParametersBuilder.build()

            this.consentInformation?.requestConsentInfoUpdate(activity,
                consentRequestParameters,
                {
                    listener.onConsentInformationRequestSuccess()
                },
                { formError ->
                    // Handle the error
                    listener.onConsentInformationRequestFailure(errorCode = formError.errorCode,
                                                                errorMessage = formError.message ?: "Unknown error")
                })
        }
    }

    fun currentConsentInformation()
        : ConsentStatusData? {
        if (this.consentInformation == null)
            return null

        val currentConsentStatus = this.consentInformation?.consentStatus ?: ConsentInformation.ConsentStatus.UNKNOWN
        val currentPrivacyOptionRequirements = this.consentInformation?.privacyOptionsRequirementStatus ?: ConsentInformation.PrivacyOptionsRequirementStatus.UNKNOWN

        val consentStatusValue = when(currentConsentStatus) {
            ConsentInformation.ConsentStatus.UNKNOWN -> 0
            ConsentInformation.ConsentStatus.NOT_REQUIRED -> 1
            ConsentInformation.ConsentStatus.REQUIRED -> 2
            ConsentInformation.ConsentStatus.OBTAINED -> 3
            else -> 0
        }

        val privacyOptionRequirementsValue = when(currentPrivacyOptionRequirements) {
            ConsentInformation.PrivacyOptionsRequirementStatus.UNKNOWN -> 0
            ConsentInformation.PrivacyOptionsRequirementStatus.REQUIRED -> 1
            ConsentInformation.PrivacyOptionsRequirementStatus.NOT_REQUIRED -> 2
        }

        val epochTimestamp = Instant.now().toEpochMilli()
        val consentStatusData = ConsentStatusData(lastRefreshTimestampMilliseconds = epochTimestamp,
                                                  consentStatus = consentStatusValue,
                                                  privacyOptionsRequirementStatus = privacyOptionRequirementsValue)

        return consentStatusData
    }

    fun showPrivacyOptionsForm(activity: Activity,
                               listener: OnConsentFormEventListener) {
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

    fun canRequestAds(): Boolean? {
        if (this.consentInformation == null)
            return null

        val canRequest = this.consentInformation?.canRequestAds()
        return canRequest
    }
}