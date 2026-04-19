package it.amdev.admob.wrapper.ads

import android.app.Activity
import android.content.Context
import com.google.android.gms.ads.AdError
import com.google.android.gms.ads.AdRequest
import com.google.android.gms.ads.FullScreenContentCallback
import com.google.android.gms.ads.LoadAdError
import com.google.android.gms.ads.appopen.AppOpenAd
import it.amdev.admob.wrapper.listeners.OnAdEventListener
import it.amdev.admob.wrapper.listeners.OnAdLoadedListener

class AppOpenAdWrapper(private val context: Context) {

    private var appOpenAd: AppOpenAd? = null
    private var isShowingAd = false

    @JvmOverloads
    fun load(
        adUnitId: String,
        loadListener: OnAdLoadedListener,
        eventListener: OnAdEventListener? = null
    ) {
        val adRequest = AdRequest.Builder().build()

        AppOpenAd.load(
            context,
            adUnitId,
            adRequest,
            object : AppOpenAd.AppOpenAdLoadCallback() {
                override fun onAdLoaded(ad: AppOpenAd) {
                    appOpenAd = ad
                    appOpenAd?.fullScreenContentCallback = object : FullScreenContentCallback() {
                        override fun onAdShowedFullScreenContent() {
                            isShowingAd = true
                            eventListener?.onAdShown()
                        }
                        override fun onAdDismissedFullScreenContent() {
                            isShowingAd = false
                            appOpenAd = null
                            eventListener?.onAdDismissed()
                        }
                        override fun onAdClicked() {
                            eventListener?.onAdClicked()
                        }
                        override fun onAdImpression() {
                            eventListener?.onAdImpression()
                        }
                        override fun onAdFailedToShowFullScreenContent(error: AdError) {
                            isShowingAd = false
                            appOpenAd = null
                            eventListener?.onAdFailedToShow(error.code, error.message)
                        }
                    }
                    loadListener.onAdLoaded()
                }

                override fun onAdFailedToLoad(error: LoadAdError) {
                    appOpenAd = null
                    loadListener.onAdFailedToLoad(error.code, error.message)
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
}