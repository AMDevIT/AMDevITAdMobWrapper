package it.amdev.admob.wrapper.ads

import android.Manifest
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
                    val width = context.resources.configuration.screenWidthDp

                    if (maxHeight != null)
                        AdSize.getInlineAdaptiveBannerAdSize(width, maxHeight)
                    else
                        AdSize.getLargeAnchoredAdaptiveBannerAdSize(context, width)
                }
            }

            setAdSize(nativeAdSize)
            adListener = createAdListener(loadListener, eventListener)
        }

        adView.loadAd(AdRequest.Builder().build())
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

        val adView = AdView(context).apply {
            this.adUnitId = adUnitId

            val nativeAdSize = if (maxHeight != null)
                AdSize.getInlineAdaptiveBannerAdSize(width, maxHeight)
            else
                AdSize.getLargeAnchoredAdaptiveBannerAdSize(context, width)

            setAdSize(nativeAdSize)
            adListener = createAdListener(loadListener, eventListener)
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

    private fun createAdListener(loadListener: OnAdLoadedListener,
                                 eventListener: OnAdEventListener?): AdListener {
        return object : AdListener() {
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
}
