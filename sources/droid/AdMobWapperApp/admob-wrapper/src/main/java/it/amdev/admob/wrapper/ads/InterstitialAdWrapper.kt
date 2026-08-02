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
class InterstitialAdWrapper(private val logger: IDroidLogger? = null) {

    private var interstitialAd: InterstitialAd? = null

    @JvmOverloads
    fun load(adUnitId: String,
             loadListener: OnAdLoadedListener,
             eventListener: OnAdEventListener? = null)
    {
        val adRequest = AdRequest.Builder(adUnitId = adUnitId)
                                 .build()

        InterstitialAd.load(adRequest,
            object : AdLoadCallback<InterstitialAd> {
                override fun onAdLoaded(ad: InterstitialAd) {
                    interstitialAd = ad
                    interstitialAd?.adEventCallback = object: InterstitialAdEventCallback {
                        override fun onAdShowedFullScreenContent() {
                            eventListener?.onAdShown()
                        }

                        override fun onAdDismissedFullScreenContent() {
                            interstitialAd = null
                            eventListener?.onAdDismissed()
                        }

                        override fun onAdClicked() {
                            eventListener?.onAdClicked()
                        }

                        override fun onAdImpression() {
                            eventListener?.onAdImpression()
                        }

                        override fun onAdFailedToShowFullScreenContent(fullScreenContentError: FullScreenContentError) {
                            interstitialAd = null

                            val errorCode = fullScreenContentError.code.toInt()
                            eventListener?.onAdFailedToShow(errorCode = errorCode,
                                                            errorMessage = fullScreenContentError.message)
                        }

                        override fun onAdPaid(value: AdValue) {
                            super.onAdPaid(value)
                            logger?.logDebug(tag = LOG_TAG,
                                             message = "onAdPaid: value = ${value.valueMicros}, " +
                                                       "currencyCode = ${value.currencyCode}, " +
                                                       "precision = ${value.precisionType}")
                        }

                        override fun onAppEvent(name: String, data: String?) {
                            super.onAppEvent(name, data)
                            logger?.logDebug(tag = LOG_TAG,
                                             message = "onAppEvent: name = $name, data = $data")
                        }
                    }
                    loadListener.onAdLoaded()
                }

                override fun onAdFailedToLoad(adError: LoadAdError) {
                    interstitialAd = null

                    val errorCode = adError.code.toInt()
                    loadListener.onAdFailedToLoad(errorCode = errorCode,
                                                  errorMessage = adError.message)
                }
            }
        )
    }

    fun show(activity: Activity, loadListener: OnAdLoadedListener? = null) {
        if (interstitialAd != null) {
            interstitialAd?.show(activity)
        } else {
            loadListener?.onAdFailedToLoad(-1, "Ad not loaded yet")
        }
    }

    fun isLoaded(): Boolean = interstitialAd != null

    companion object {
        private const val LOG_TAG = "InterstitialAdWrapper"
    }
}