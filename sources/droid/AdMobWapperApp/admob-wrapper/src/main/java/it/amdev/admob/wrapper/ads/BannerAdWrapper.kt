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
                      private val logger: IDroidLogger? = null) {

    private var bannerView: View? = null

    @RequiresPermission(Manifest.permission.INTERNET)
    @JvmOverloads
    fun load(adUnitId: String,
             adSize: BannerAdViewSize = BannerAdViewSize.Banner,
             loadListener: OnAdLoadedListener,
             eventListener: OnAdEventListener? = null,
             maxHeight:  Int? = null)
        : View
    {
        if (this.bannerView is AdView) {
            (this.bannerView as AdView).destroy()
        }

        val nativeAdSize = when(adSize) {
            BannerAdViewSize.Banner -> AdSize.BANNER
            BannerAdViewSize.LargeBanner -> AdSize.LARGE_BANNER
            BannerAdViewSize.MediumRectangle -> AdSize.MEDIUM_RECTANGLE
            BannerAdViewSize.FullBanner -> AdSize.FULL_BANNER
            BannerAdViewSize.Leaderboard -> AdSize.LEADERBOARD
            BannerAdViewSize.Adaptive -> {
                val width = context.resources.configuration.screenWidthDp

                if (maxHeight != null)
                    AdSize.getInlineAdaptiveBannerAdSize(width, maxHeight)
                else
                    AdSize.getLargeAnchoredAdaptiveBannerAdSize(context, width)
            }
        }

        val adView = AdView(context).apply {
            // Nothing to apply right now
        }

        val adRequest = BannerAdRequest.Builder(adUnitId = adUnitId,
                                                adSize = nativeAdSize)
                                       .build()
        adView.loadAd(adRequest = adRequest,
            object : AdLoadCallback<BannerAd> {
                override fun onAdLoaded(ad: BannerAd) {
                    loadListener.onAdLoaded()
                    ad.adEventCallback = object : BannerAdEventCallback {
                        override fun onAdClicked() {
                            eventListener?.onAdClicked()
                        }

                        override fun onAdImpression() {
                            eventListener?.onAdImpression()
                        }

                        override fun onAdDismissedFullScreenContent() {
                            eventListener?.onAdDismissed()
                        }

                        override fun onAdPaid(value: AdValue) {
                            super.onAdPaid(value)
                            logger?.logDebug(tag = LOG_TAG,
                                             message = "onAdPaid: value = ${value.valueMicros}, " +
                                                       "currencyCode = ${value.currencyCode}, " +
                                                       "precision = ${value.precisionType}")
                        }

                        override fun onAdShowedFullScreenContent() {
                            eventListener?.onAdShown()
                        }

                        override fun onAdFailedToShowFullScreenContent(fullScreenContentError: FullScreenContentError) {
                            val errorCode = fullScreenContentError.code.toInt()
                            eventListener?.onAdFailedToShow(errorCode = errorCode, errorMessage = fullScreenContentError.message)
                        }

                        override fun onAppEvent(name: String, data: String?) {
                            super.onAppEvent(name, data)
                        }
                    }

                    ad.bannerAdRefreshCallback = object : BannerAdRefreshCallback {
                        override fun onAdRefreshed() {
                            super.onAdRefreshed()
                            logger?.logDebug(tag = LOG_TAG, message = "Ad refreshed")
                        }

                        override fun onAdFailedToRefresh(adError: LoadAdError) {
                            super.onAdFailedToRefresh(adError)
                            logger?.logError(tag = LOG_TAG, message = "Ad failed to refresh: ${adError.message}")
                        }
                    }
                }

                override fun onAdFailedToLoad(adError: LoadAdError) {
                    val errorCode = adError.code.toInt()
                    loadListener.onAdFailedToLoad(errorCode = errorCode,
                                                  errorMessage = adError.message)
                }
            })
        bannerView = adView
        return adView
    }

    @RequiresPermission(Manifest.permission.INTERNET)
    @JvmOverloads
    fun loadAdaptive(adUnitId: String,
                     width: Int,
                     loadListener: OnAdLoadedListener,
                     eventListener: OnAdEventListener? = null,
                     maxHeight: Int? = null)
        : View
    {
        require(width > 0) { "Adaptive banner width must be greater than zero" }

        if (this.bannerView is AdView) {
            (this.bannerView as AdView).destroy()
        }

        val nativeAdSize = if (maxHeight != null)
            AdSize.getInlineAdaptiveBannerAdSize(width, maxHeight)
        else
            AdSize.getLargeAnchoredAdaptiveBannerAdSize(context, width)

        val adView = AdView(context).apply {
        }

        val adRequest = BannerAdRequest.Builder(adUnitId = adUnitId,
                                                adSize = nativeAdSize)
                                       .build()
        adView.loadAd(adRequest = adRequest,
            object : AdLoadCallback<BannerAd> {
                override fun onAdLoaded(ad: BannerAd) {
                    loadListener.onAdLoaded()
                    ad.adEventCallback = object : BannerAdEventCallback {
                        override fun onAdClicked() {
                            eventListener?.onAdClicked()
                        }

                        override fun onAdImpression() {
                            eventListener?.onAdImpression()
                        }

                        override fun onAdDismissedFullScreenContent() {
                            eventListener?.onAdDismissed()
                        }

                        override fun onAdPaid(value: AdValue) {
                            super.onAdPaid(value)

                            logger?.logDebug(tag = LOG_TAG,
                                             message = "onAdPaid: value = ${value.valueMicros}, " +
                                                       "currencyCode = ${value.currencyCode}, " +
                                                       "precision = ${value.precisionType}")
                        }

                        override fun onAdShowedFullScreenContent() {
                            eventListener?.onAdShown()
                        }

                        override fun onAdFailedToShowFullScreenContent(fullScreenContentError: FullScreenContentError) {
                            val errorCode = fullScreenContentError.code.toInt()
                            eventListener?.onAdFailedToShow(errorCode = errorCode, errorMessage = fullScreenContentError.message)
                        }

                        override fun onAppEvent(name: String, data: String?) {
                            super.onAppEvent(name, data)

                            logger?.logDebug(tag = LOG_TAG,
                                             message = "onAppEvent: name = $name, data = $data")
                        }
                    }

                    ad.bannerAdRefreshCallback = object : BannerAdRefreshCallback {
                        override fun onAdRefreshed() {
                            super.onAdRefreshed()
                            logger?.logDebug(tag = LOG_TAG, message = "Ad refreshed")
                        }

                        override fun onAdFailedToRefresh(adError: LoadAdError) {
                            super.onAdFailedToRefresh(adError)
                            logger?.logError(tag = LOG_TAG, message = "Ad failed to refresh: ${adError.message}")
                        }
                    }
                }

                override fun onAdFailedToLoad(adError: LoadAdError) {
                    val errorCode = adError.code.toInt()
                    loadListener.onAdFailedToLoad(errorCode = errorCode,
                        errorMessage = adError.message)
                }
            })
        bannerView = adView
        return adView
    }

    fun destroy() {
        if (this.bannerView is AdView) {
            (this.bannerView as AdView).destroy()
        }
        bannerView = null
    }

    companion object {
        private const val LOG_TAG = "BannerAdWrapper"
    }

//    private fun createAdListener(loadListener: OnAdLoadedListener,
//                                 eventListener: OnAdEventListener?): AdListener {
//        return object : AdListener() {
//            override fun onAdLoaded() {
//                loadListener.onAdLoaded()
//            }
//
//            override fun onAdFailedToLoad(error: LoadAdError) {
//                loadListener.onAdFailedToLoad(error.code, error.message)
//            }
//
//            override fun onAdOpened() {
//                eventListener?.onAdShown()
//            }
//
//            override fun onAdClosed() {
//                eventListener?.onAdDismissed()
//            }
//
//            override fun onAdClicked() {
//                eventListener?.onAdClicked()
//            }
//
//            override fun onAdImpression() {
//                eventListener?.onAdImpression()
//            }
//        }
//    }
}
