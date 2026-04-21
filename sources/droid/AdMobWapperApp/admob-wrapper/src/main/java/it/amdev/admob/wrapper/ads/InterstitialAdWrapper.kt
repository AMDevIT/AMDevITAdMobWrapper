package it.amdev.admob.wrapper.ads

import android.app.Activity
import android.content.Context
import com.google.android.gms.ads.AdError
import com.google.android.gms.ads.AdRequest
import com.google.android.gms.ads.FullScreenContentCallback
import com.google.android.gms.ads.LoadAdError
import com.google.android.gms.ads.interstitial.InterstitialAd
import com.google.android.gms.ads.interstitial.InterstitialAdLoadCallback
import it.amdev.admob.wrapper.listeners.OnAdEventListener
import it.amdev.admob.wrapper.listeners.OnAdLoadedListener

class InterstitialAdWrapper(private val context: Context) {

    private var interstitialAd: InterstitialAd? = null

    @JvmOverloads
    fun load(adUnitId: String,
             loadListener: OnAdLoadedListener,
             eventListener: OnAdEventListener? = null)
    {
        val adRequest = AdRequest.Builder().build()

        InterstitialAd.load(context,
                            adUnitId,
                            adRequest,
            object : InterstitialAdLoadCallback() {
                override fun onAdLoaded(ad: InterstitialAd) {
                    interstitialAd = ad
                    interstitialAd?.fullScreenContentCallback = object : FullScreenContentCallback() {
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
                        override fun onAdFailedToShowFullScreenContent(error: AdError) {
                            interstitialAd = null
                            eventListener?.onAdFailedToShow(error.code,
                                                          error.message)
                        }
                    }
                    loadListener.onAdLoaded()
                }

                override fun onAdFailedToLoad(error: LoadAdError) {
                    interstitialAd = null
                    loadListener.onAdFailedToLoad(error.code, error.message)
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
}