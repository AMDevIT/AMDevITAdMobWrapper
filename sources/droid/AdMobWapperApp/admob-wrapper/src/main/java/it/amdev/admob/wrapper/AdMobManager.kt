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
import it.amdev.admob.wrapper.listeners.OnInitializedListener
import it.amdev.admob.wrapper.privacy.ConsentInformationRequestDebugParameters

@Suppress("unused")
class AdMobManager(private val logger: IDroidLogger? = null) {

    private var initialized = false
    private var consentInformation: ConsentInformation? = null

    companion object {
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

                },
                {

                })
        }
    }

    fun currentConsentInformation() {

    }
}