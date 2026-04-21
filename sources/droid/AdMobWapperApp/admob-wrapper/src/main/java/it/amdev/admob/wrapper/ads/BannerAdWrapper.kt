package it.amdev.admob.wrapper.ads

import android.Manifest
import android.app.Activity
import android.content.Context
import android.view.View
import androidx.annotation.RequiresPermission
import com.google.android.gms.ads.AdListener
import com.google.android.gms.ads.AdRequest
import com.google.android.gms.ads.AdSize
import com.google.android.gms.ads.AdView
import com.google.android.gms.ads.LoadAdError
import it.amdev.admob.wrapper.listeners.OnAdEventListener
import it.amdev.admob.wrapper.listeners.OnAdLoadedListener

@Suppress("unused")
class BannerAdWrapper(private val context: Context) {

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
        val adView = AdView(context).apply {
            this.adUnitId = adUnitId

            val nativeAdSize = when(adSize) {
                BannerAdViewSize.Banner -> AdSize.BANNER
                BannerAdViewSize.LargeBanner -> AdSize.LARGE_BANNER
                BannerAdViewSize.MediumRectangle -> AdSize.MEDIUM_RECTANGLE
                BannerAdViewSize.FullBanner -> AdSize.FULL_BANNER
                BannerAdViewSize.Leaderboard -> AdSize.LEADERBOARD
                BannerAdViewSize.Adaptive -> {
                    val activity = context as? Activity
                        ?: throw IllegalArgumentException("Context must be an Activity for Adaptive banner size")
                    val windowMetrics = activity.windowManager.currentWindowMetrics
                    val bounds = windowMetrics.bounds
                    val density = context.resources.displayMetrics.density
                    val width = (bounds.width() / density).toInt()

                    if (maxHeight != null)
                        AdSize.getInlineAdaptiveBannerAdSize(width, maxHeight)
                    else
                        AdSize.getCurrentOrientationInlineAdaptiveBannerAdSize(context, width)
                }
            }

            setAdSize(nativeAdSize)
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
        if (this.bannerView is AdView) {
            (this.bannerView as AdView).destroy()
        }
        bannerView = null
    }
}