package it.amdev.admob.wrapper

import android.content.Context
import com.google.android.gms.ads.MobileAds
import it.amdev.admob.wrapper.listeners.OnInitializedListener

class AdMobManager private constructor() {
    private var initialized = false

    companion object {
        @JvmStatic
        val instance: AdMobManager by lazy { AdMobManager() }
    }

    fun initialize(context: Context, listener: OnInitializedListener) {
        if (initialized) {
            listener.onInitialized()
            return
        }
        Thread {
            try {
                MobileAds.initialize(context) {
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