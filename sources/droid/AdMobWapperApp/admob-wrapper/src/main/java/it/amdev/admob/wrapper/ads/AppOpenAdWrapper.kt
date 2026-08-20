package it.amdev.admob.wrapper.ads

import android.app.Activity
import com.google.android.libraries.ads.mobile.sdk.appopen.AppOpenAd
import com.google.android.libraries.ads.mobile.sdk.appopen.AppOpenAdEventCallback
import com.google.android.libraries.ads.mobile.sdk.common.AdLoadCallback
import com.google.android.libraries.ads.mobile.sdk.common.AdRequest
import com.google.android.libraries.ads.mobile.sdk.common.AdValue
import com.google.android.libraries.ads.mobile.sdk.common.FullScreenContentError
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError
import it.amdev.admob.wrapper.diagnostics.IDroidLogger
import it.amdev.admob.wrapper.listeners.OnAdEventListener
import it.amdev.admob.wrapper.listeners.OnAdLoadedListener
import it.amdev.admob.wrapper.utils.ErrorsObjectsExtensions.Companion.toInt

@Suppress("unused")
class AppOpenAdWrapper(logger: IDroidLogger? = null) {

    private val lifecycle = CallbackLifecycle()
    private var generation = 0L
    private var appOpenAd: AppOpenAd? = null
    private var isShowingAd = false
    private var loadListener: OnAdLoadedListener? = null
    private var eventListener: OnAdEventListener? = null
    private var logger: IDroidLogger? = logger

    @JvmOverloads
    fun load(adUnitId: String,
             loadListener: OnAdLoadedListener,
             eventListener: OnAdEventListener? = null) {
        this.lifecycle.begin { generation ->
            this.generation = generation
            this.clearAdLocked()
            this.loadListener = loadListener
            this.eventListener = eventListener
            this.isShowingAd = false

            val adRequest = AdRequest.Builder(adUnitId = adUnitId).build()
            AppOpenAd.load(adRequest = adRequest,
                           adLoadCallback = object : AdLoadCallback<AppOpenAd> {
                               override fun onAdLoaded(ad: AppOpenAd) {
                                   val accepted = lifecycle.access(generation) {
                                       appOpenAd = ad
                                       ad.adEventCallback = createEventCallback(generation)
                                       true
                                   } == true

                                   if (!accepted) {
                                       clearCallback(ad)
                                       return
                                   }

                                   notifyLoaded(generation)
                               }

                               override fun onAdFailedToLoad(adError: LoadAdError) {
                                   lifecycle.access(generation) { appOpenAd = null }
                                   notifyLoadFailed(generation,
                                                    adError.code.toInt(),
                                                    adError.message)
                               }
                           })
        }
    }

    fun show(activity: Activity, loadListener: OnAdLoadedListener? = null) {
        this.lifecycle.access(this.generation) {
            if (this.isShowingAd)
                return@access

            val ad = this.appOpenAd
            if (ad != null) {
                ad.show(activity)
            } else {
                loadListener?.let { listener ->
                    this.invokeSafely { listener.onAdFailedToLoad(-1, "Ad not loaded yet") }
                }
            }
        }
    }

    fun isLoaded(): Boolean {
        return this.lifecycle.access(this.generation) { this.appOpenAd != null } == true
    }

    fun isShowing(): Boolean {
        return this.lifecycle.access(this.generation) { this.isShowingAd } == true
    }

    fun destroy() {
        this.lifecycle.destroy {
            this.loadListener = null
            this.eventListener = null
            this.logger = null
            this.isShowingAd = false
            this.clearAdLocked()
        }
    }

    private fun createEventCallback(generation: Long): AppOpenAdEventCallback {
        return object : AppOpenAdEventCallback {
            override fun onAdDismissedFullScreenContent() {
                this@AppOpenAdWrapper.lifecycle.access(generation) {
                    isShowingAd = false
                    appOpenAd = null
                }
                this@AppOpenAdWrapper.notifyEvent(generation) { it.onAdDismissed() }
            }

            override fun onAdFailedToShowFullScreenContent(fullScreenContentError: FullScreenContentError) {
                this@AppOpenAdWrapper.lifecycle.access(generation) {
                    isShowingAd = false
                    appOpenAd = null
                }
                this@AppOpenAdWrapper.notifyEvent(generation) {
                    it.onAdFailedToShow(errorCode = fullScreenContentError.code.toInt(),
                                        errorMessage = fullScreenContentError.message)
                }
            }

            override fun onAdShowedFullScreenContent() {
                this@AppOpenAdWrapper.lifecycle.access(generation) { isShowingAd = true }
                this@AppOpenAdWrapper.notifyEvent(generation) { it.onAdShown() }
            }

            override fun onAdClicked() {
                this@AppOpenAdWrapper.notifyEvent(generation) { it.onAdClicked() }
            }

            override fun onAdImpression() {
                this@AppOpenAdWrapper.notifyEvent(generation) { it.onAdImpression() }
            }

            override fun onAdPaid(value: AdValue) {
                super.onAdPaid(value)
                this@AppOpenAdWrapper.logDebug(generation,
                                               "Ad paid: ${value.valueMicros} ${value.currencyCode}")
            }
        }
    }

    private fun clearAdLocked() {
        this.appOpenAd?.let { this.clearCallback(it) }
        this.appOpenAd = null
    }

    private fun clearCallback(ad: AppOpenAd) {
        try {
            ad.adEventCallback = NO_OP_EVENT_CALLBACK
        } catch (_: Throwable) {
            // Native callback cleanup must never make teardown fail.
        }
    }

    private fun notifyLoaded(generation: Long) {
        this.lifecycle.access(generation) {
            this.loadListener?.let { listener -> this.invokeSafely { listener.onAdLoaded() } }
        }
    }

    private fun notifyLoadFailed(generation: Long, errorCode: Int, errorMessage: String) {
        this.lifecycle.access(generation) {
            this.loadListener?.let { listener ->
                this.invokeSafely {
                    listener.onAdFailedToLoad(errorCode = errorCode,
                                              errorMessage = errorMessage)
                }
            }
        }
    }

    private fun notifyEvent(generation: Long, callback: (OnAdEventListener) -> Unit) {
        this.lifecycle.access(generation) {
            this.eventListener?.let { listener -> this.invokeSafely { callback(listener) } }
        }
    }

    private fun logDebug(generation: Long, message: String) {
        this.lifecycle.access(generation) {
            this.logger?.let { activeLogger ->
                this.invokeSafely { activeLogger.logDebug(tag = LOG_TAG, message = message) }
            }
        }
    }

    private fun invokeSafely(callback: () -> Unit) {
        try {
            callback()
        } catch (_: Throwable) {
            // Managed diagnostics and listeners must never crash a native callback thread.
        }
    }

    companion object {
        private const val LOG_TAG = "AppOpenAdWrapper"
        private val NO_OP_EVENT_CALLBACK = object : AppOpenAdEventCallback {}
    }
}
