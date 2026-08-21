package it.amdev.admob.wrapper.ads

import android.Manifest
import android.content.Context
import android.view.View
import androidx.annotation.RequiresPermission
import com.google.android.libraries.ads.mobile.sdk.banner.AdSize
import com.google.android.libraries.ads.mobile.sdk.banner.AdView
import com.google.android.libraries.ads.mobile.sdk.banner.BannerAd
import com.google.android.libraries.ads.mobile.sdk.banner.BannerAdEventCallback
import com.google.android.libraries.ads.mobile.sdk.banner.BannerAdRefreshCallback
import com.google.android.libraries.ads.mobile.sdk.banner.BannerAdRequest
import com.google.android.libraries.ads.mobile.sdk.common.AdLoadCallback
import com.google.android.libraries.ads.mobile.sdk.common.AdValue
import com.google.android.libraries.ads.mobile.sdk.common.FullScreenContentError
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError
import it.amdev.admob.wrapper.diagnostics.IDroidLogger
import it.amdev.admob.wrapper.listeners.OnAdEventListener
import it.amdev.admob.wrapper.listeners.OnAdLoadedListener
import it.amdev.admob.wrapper.utils.ErrorsObjectsExtensions.Companion.toInt

@Suppress("unused")
class BannerAdWrapper(private val context: Context,
                      logger: IDroidLogger? = null) {

    private val lifecycle = CallbackLifecycle()
    private var bannerView: AdView? = null
    private var bannerAd: BannerAd? = null
    private var loadListener: OnAdLoadedListener? = null
    private var eventListener: OnAdEventListener? = null
    private var logger: IDroidLogger? = logger

    @RequiresPermission(Manifest.permission.INTERNET)
    @JvmOverloads
    fun load(adUnitId: String,
             adSize: BannerAdViewSize = BannerAdViewSize.Banner,
             loadListener: OnAdLoadedListener,
             eventListener: OnAdEventListener? = null,
             maxHeight: Int? = null): View {
        val nativeAdSize = when (adSize) {
            BannerAdViewSize.Banner -> AdSize.BANNER
            BannerAdViewSize.LargeBanner -> AdSize.LARGE_BANNER
            BannerAdViewSize.MediumRectangle -> AdSize.MEDIUM_RECTANGLE
            BannerAdViewSize.FullBanner -> AdSize.FULL_BANNER
            BannerAdViewSize.Leaderboard -> AdSize.LEADERBOARD
            BannerAdViewSize.Adaptive -> {
                val width = this.context.resources.configuration.screenWidthDp

                if (maxHeight != null)
                    AdSize.getInlineAdaptiveBannerAdSize(width, maxHeight)
                else
                    AdSize.getLargeAnchoredAdaptiveBannerAdSize(this.context, width)
            }
        }

        return this.load(adUnitId,
                         nativeAdSize,
                         loadListener,
                         eventListener)
    }

    @RequiresPermission(Manifest.permission.INTERNET)
    @JvmOverloads
    fun loadAdaptive(adUnitId: String,
                     width: Int,
                     loadListener: OnAdLoadedListener,
                     eventListener: OnAdEventListener? = null,
                     maxHeight: Int? = null): View {
        require(width > 0) { "Adaptive banner width must be greater than zero" }

        val nativeAdSize = if (maxHeight != null)
            AdSize.getInlineAdaptiveBannerAdSize(width, maxHeight)
        else
            AdSize.getLargeAnchoredAdaptiveBannerAdSize(this.context, width)

        return this.load(adUnitId,
                         nativeAdSize,
                         loadListener,
                         eventListener)
    }

    fun destroy() {
        this.lifecycle.destroy {
            this.loadListener = null
            this.eventListener = null
            this.logger = null
            this.clearBannerLocked()
        }
    }

    @RequiresPermission(Manifest.permission.INTERNET)
    private fun load(adUnitId: String,
                     adSize: AdSize,
                     loadListener: OnAdLoadedListener,
                     eventListener: OnAdEventListener?): View {
        return this.lifecycle.begin { generation ->
            this.clearBannerLocked()
            this.loadListener = loadListener
            this.eventListener = eventListener

            val adView = AdView(this.context)
            val adRequest = BannerAdRequest.Builder(adUnitId = adUnitId,
                                                    adSize = adSize)
                                           .build()

            this.bannerView = adView
            adView.loadAd(adRequest = adRequest,
                          object : AdLoadCallback<BannerAd> {
                              override fun onAdLoaded(ad: BannerAd) {
                                  val accepted = lifecycle.access(generation) {
                                      bannerAd = ad
                                      ad.adEventCallback = createEventCallback(generation)
                                      ad.bannerAdRefreshCallback = createRefreshCallback(generation)
                                      true
                                  } == true

                                  if (!accepted) {
                                      clearCallbacks(ad)
                                      return
                                  }

                                  notifyLoaded(generation)
                              }

                              override fun onAdFailedToLoad(adError: LoadAdError) {
                                  notifyLoadFailed(generation,
                                                   adError.code.toInt(),
                                                   adError.message)
                              }
                          })
            adView
        }
    }

    private fun createEventCallback(generation: Long): BannerAdEventCallback {
        return object : BannerAdEventCallback {
            override fun onAdClicked() {
                this@BannerAdWrapper.notifyEvent(generation) { it.onAdClicked() }
            }

            override fun onAdImpression() {
                this@BannerAdWrapper.notifyEvent(generation) { it.onAdImpression() }
            }

            override fun onAdDismissedFullScreenContent() {
                this@BannerAdWrapper.notifyEvent(generation) { it.onAdDismissed() }
            }

            override fun onAdPaid(value: AdValue) {
                super.onAdPaid(value)
                this@BannerAdWrapper.logDebug(generation,
                                              "onAdPaid: value = ${value.valueMicros}, " +
                                                  "currencyCode = ${value.currencyCode}, " +
                                                  "precision = ${value.precisionType}")
            }

            override fun onAdShowedFullScreenContent() {
                this@BannerAdWrapper.notifyEvent(generation) { it.onAdShown() }
            }

            override fun onAdFailedToShowFullScreenContent(fullScreenContentError: FullScreenContentError) {
                val errorCode = fullScreenContentError.code.toInt()
                this@BannerAdWrapper.notifyEvent(generation) {
                    it.onAdFailedToShow(errorCode = errorCode,
                                        errorMessage = fullScreenContentError.message)
                }
            }

            override fun onAppEvent(name: String, data: String?) {
                super.onAppEvent(name, data)
                this@BannerAdWrapper.logDebug(generation,
                                              "onAppEvent: name = $name, data = $data")
            }
        }
    }

    private fun createRefreshCallback(generation: Long): BannerAdRefreshCallback {
        return object : BannerAdRefreshCallback {
            override fun onAdRefreshed() {
                super.onAdRefreshed()
                this@BannerAdWrapper.logDebug(generation, "Ad refreshed")
            }

            override fun onAdFailedToRefresh(adError: LoadAdError) {
                super.onAdFailedToRefresh(adError)
                this@BannerAdWrapper.logError(generation,
                                              "Ad failed to refresh: ${adError.message}")
            }
        }
    }

    private fun clearBannerLocked() {
        this.bannerAd?.let { this.clearCallbacks(it) }
        this.bannerAd = null
        val view = this.bannerView
        this.bannerView = null

        try {
            view?.destroy()
        } catch (_: Throwable) {
            // Native view teardown must remain idempotent and non-fatal.
        }
    }

    private fun clearCallbacks(ad: BannerAd) {
        try {
            ad.adEventCallback = NO_OP_EVENT_CALLBACK
            ad.bannerAdRefreshCallback = NO_OP_REFRESH_CALLBACK
        } catch (_: Throwable) {
            // Native callback cleanup must never make teardown fail.
        }
    }

    private fun notifyLoaded(generation: Long) {
        this.lifecycle.access(generation) {
            this.loadListener?.let { listener ->
                this.invokeSafely { listener.onAdLoaded() }
            }
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
            this.eventListener?.let { listener ->
                this.invokeSafely { callback(listener) }
            }
        }
    }

    private fun logDebug(generation: Long, message: String) {
        this.lifecycle.access(generation) {
            this.logger?.let { activeLogger ->
                this.invokeSafely { activeLogger.logDebug(tag = LOG_TAG, message = message) }
            }
        }
    }

    private fun logError(generation: Long, message: String) {
        this.lifecycle.access(generation) {
            this.logger?.let { activeLogger ->
                this.invokeSafely { activeLogger.logError(tag = LOG_TAG, message = message) }
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
        private const val LOG_TAG = "BannerAdWrapper"

        private val NO_OP_EVENT_CALLBACK = object : BannerAdEventCallback {}
        private val NO_OP_REFRESH_CALLBACK = object : BannerAdRefreshCallback {}
    }
}
