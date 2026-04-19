package it.amdev.admob.wrapper.ads

import android.Manifest
import android.content.Context
import androidx.annotation.RequiresPermission
import com.google.android.gms.ads.AdListener
import com.google.android.gms.ads.AdRequest
import com.google.android.gms.ads.AdSize
import com.google.android.gms.ads.AdView
import com.google.android.gms.ads.LoadAdError
import it.amdev.admob.wrapper.listeners.OnAdEventListener
import it.amdev.admob.wrapper.listeners.OnAdLoadedListener

class BannerAdWrapper(private val context: Context) {

    private var bannerView: AdView? = null

    @RequiresPermission(Manifest.permission.INTERNET)
    @JvmOverloads
    fun load(
        adUnitId: String,
        adSize: AdSize = AdSize.BANNER,
        loadListener: OnAdLoadedListener,
        eventListener: OnAdEventListener? = null
    ): AdView {
        bannerView?.destroy()

        val adView = AdView(context).apply {
            this.adUnitId = adUnitId
            setAdSize(adSize)
            adListener = object : AdListener() {
                override fun onAdLoaded() {
                    loadListener.onAdLoaded()
                }
                override fun onAdFailedToLoad(error: LoadAdError) {
                    loadListener.onAdFailedToLoad(error.code, error.message)
                }
                override fun onAdOpened() {
                    eventListener?.onAdShown()
                }
                override fun onAdClosed() {
                    eventListener?.onAdDismissed()
                }
                override fun onAdClicked() {
                    eventListener?.onAdClicked()
                }
                override fun onAdImpression() {
                    eventListener?.onAdImpression()
                }
            }
        }

        adView.loadAd(AdRequest.Builder().build())
        bannerView = adView
        return adView
    }

    fun destroy() {
        bannerView?.destroy()
        bannerView = null
    }
}