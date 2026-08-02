package it.amdev.admob.wrapper

import android.content.Context
import com.google.android.libraries.ads.mobile.sdk.MobileAds
import com.google.android.libraries.ads.mobile.sdk.initialization.InitializationConfig
import it.amdev.admob.wrapper.listeners.OnInitializedListener

@Suppress("unused")
class AdMobManager private constructor() {
    private var initialized = false

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
}