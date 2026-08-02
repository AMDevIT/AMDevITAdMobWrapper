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
class AppOpenAdWrapper(private val logger: IDroidLogger? = null) {

    private var appOpenAd: AppOpenAd? = null
    private var isShowingAd = false

    @JvmOverloads
    fun load(adUnitId: String,
             loadListener: OnAdLoadedListener,
             eventListener: OnAdEventListener? = null)
    {
        val adRequest = AdRequest.Builder(adUnitId = adUnitId).build()

        AppOpenAd.load(adRequest = adRequest,
                       adLoadCallback = object : AdLoadCallback<AppOpenAd> {
                override fun onAdLoaded(ad: AppOpenAd) {
                    appOpenAd = ad
                    appOpenAd?.adEventCallback = object : AppOpenAdEventCallback {
                        override fun onAdDismissedFullScreenContent() {
                            isShowingAd = false
                            appOpenAd = null
                            eventListener?.onAdDismissed()
                        }

                        override fun onAdFailedToShowFullScreenContent(fullScreenContentError: FullScreenContentError) {
                            isShowingAd = false
                            appOpenAd = null
                            val errorCode = fullScreenContentError.code.toInt()
                            eventListener?.onAdFailedToShow(errorCode = errorCode,
                                                            errorMessage = fullScreenContentError.message)
                        }

                        override fun onAdShowedFullScreenContent() {
                            isShowingAd = true
                            eventListener?.onAdShown()
                        }
                        override fun onAdClicked() {
                            eventListener?.onAdClicked()
                        }
                        override fun onAdImpression() {
                            eventListener?.onAdImpression()
                        }

                        override fun onAdPaid(value: AdValue) {
                            super.onAdPaid(value)
                            logger?.logDebug(tag = LOG_TAG,
                                             message = "Ad paid: ${value.valueMicros} ${value.currencyCode}")
                        }
                    }
                    loadListener.onAdLoaded()
                }

                override fun onAdFailedToLoad(adError: LoadAdError) {
                    appOpenAd = null
                    val errorCode = adError.code.toInt()
                    loadListener.onAdFailedToLoad(errorCode =  errorCode,
                                                  errorMessage =  adError.message)
                }
            }
        )
    }

    fun show(activity: Activity, loadListener: OnAdLoadedListener? = null) {
        if (isShowingAd) return

        if (appOpenAd != null) {
            appOpenAd?.show(activity)
        } else {
            loadListener?.onAdFailedToLoad(-1, "Ad not loaded yet")
        }
    }

    fun isLoaded(): Boolean = appOpenAd != null
    fun isShowing(): Boolean = isShowingAd

    companion object {
        private const val LOG_TAG = "AppOpenAdWrapper"
    }
}