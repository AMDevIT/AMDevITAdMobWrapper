package it.amdev.admob.wrapper.ads

import android.app.Activity
import com.google.android.libraries.ads.mobile.sdk.common.AdLoadCallback
import com.google.android.libraries.ads.mobile.sdk.common.AdRequest
import com.google.android.libraries.ads.mobile.sdk.common.AdValue
import com.google.android.libraries.ads.mobile.sdk.common.FullScreenContentError
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError
import com.google.android.libraries.ads.mobile.sdk.interstitial.InterstitialAd
import com.google.android.libraries.ads.mobile.sdk.interstitial.InterstitialAdEventCallback
import it.amdev.admob.wrapper.diagnostics.IDroidLogger
import it.amdev.admob.wrapper.listeners.OnAdEventListener
import it.amdev.admob.wrapper.listeners.OnAdLoadedListener
import it.amdev.admob.wrapper.utils.ErrorsObjectsExtensions.Companion.toInt

@Suppress("unused")
class InterstitialAdWrapper(logger: IDroidLogger? = null) {

    private val lifecycle = CallbackLifecycle()
    private var generation = 0L
    private var interstitialAd: InterstitialAd? = null
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

            val adRequest = AdRequest.Builder(adUnitId = adUnitId).build()
            InterstitialAd.load(adRequest,
                                object : AdLoadCallback<InterstitialAd> {
                                    override fun onAdLoaded(ad: InterstitialAd) {
                                        val accepted = lifecycle.access(generation) {
                                            interstitialAd = ad
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
                                        lifecycle.access(generation) { interstitialAd = null }
                                        notifyLoadFailed(generation,
                                                         adError.code.toInt(),
                                                         adError.message)
                                    }
                                })
        }
    }

    fun show(activity: Activity, loadListener: OnAdLoadedListener? = null) {
        this.lifecycle.access(this.generation) {
            val ad = this.interstitialAd
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
        return this.lifecycle.access(this.generation) { this.interstitialAd != null } == true
    }

    fun destroy() {
        this.lifecycle.destroy {
            this.loadListener = null
            this.eventListener = null
            this.logger = null
            this.clearAdLocked()
        }
    }

    private fun createEventCallback(generation: Long): InterstitialAdEventCallback {
        return object : InterstitialAdEventCallback {
            override fun onAdShowedFullScreenContent() {
                this@InterstitialAdWrapper.notifyEvent(generation) { it.onAdShown() }
            }

            override fun onAdDismissedFullScreenContent() {
                this@InterstitialAdWrapper.lifecycle.access(generation) { interstitialAd = null }
                this@InterstitialAdWrapper.notifyEvent(generation) { it.onAdDismissed() }
            }

            override fun onAdClicked() {
                this@InterstitialAdWrapper.notifyEvent(generation) { it.onAdClicked() }
            }

            override fun onAdImpression() {
                this@InterstitialAdWrapper.notifyEvent(generation) { it.onAdImpression() }
            }

            override fun onAdFailedToShowFullScreenContent(fullScreenContentError: FullScreenContentError) {
                this@InterstitialAdWrapper.lifecycle.access(generation) { interstitialAd = null }
                this@InterstitialAdWrapper.notifyEvent(generation) {
                    it.onAdFailedToShow(errorCode = fullScreenContentError.code.toInt(),
                                        errorMessage = fullScreenContentError.message)
                }
            }

            override fun onAdPaid(value: AdValue) {
                super.onAdPaid(value)
                this@InterstitialAdWrapper.logDebug(generation,
                                                    "onAdPaid: value = ${value.valueMicros}, " +
                                                        "currencyCode = ${value.currencyCode}, " +
                                                        "precision = ${value.precisionType}")
            }

            override fun onAppEvent(name: String, data: String?) {
                super.onAppEvent(name, data)
                this@InterstitialAdWrapper.logDebug(generation,
                                                    "onAppEvent: name = $name, data = $data")
            }
        }
    }

    private fun clearAdLocked() {
        this.interstitialAd?.let { this.clearCallback(it) }
        this.interstitialAd = null
    }

    private fun clearCallback(ad: InterstitialAd) {
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
        private const val LOG_TAG = "InterstitialAdWrapper"
        private val NO_OP_EVENT_CALLBACK = object : InterstitialAdEventCallback {}
    }
}
