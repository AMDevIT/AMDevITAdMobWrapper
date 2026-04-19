package it.amdev.admob.wrapper.ads

import android.app.Activity
import android.content.Context
import com.google.android.gms.ads.AdError
import com.google.android.gms.ads.AdRequest
import com.google.android.gms.ads.FullScreenContentCallback
import com.google.android.gms.ads.LoadAdError
import com.google.android.gms.ads.rewarded.RewardedAd
import com.google.android.gms.ads.rewarded.RewardedAdLoadCallback
import it.amdev.admob.wrapper.listeners.OnAdEventListener
import it.amdev.admob.wrapper.listeners.OnAdLoadedListener
import it.amdev.admob.wrapper.listeners.OnRewardEarnedListener

class RewardedAdWrapper(private val context: Context) {

    private var rewardedAd: RewardedAd? = null

    @JvmOverloads
    fun load(
        adUnitId: String,
        loadListener: OnAdLoadedListener,
        eventListener: OnAdEventListener? = null
    ) {
        val adRequest = AdRequest.Builder().build()

        RewardedAd.load(
            context,
            adUnitId,
            adRequest,
            object : RewardedAdLoadCallback() {
                override fun onAdLoaded(ad: RewardedAd) {
                    rewardedAd = ad
                    rewardedAd?.fullScreenContentCallback = object : FullScreenContentCallback() {
                        override fun onAdShowedFullScreenContent() {
                            eventListener?.onAdShown()
                        }
                        override fun onAdDismissedFullScreenContent() {
                            rewardedAd = null
                            eventListener?.onAdDismissed()
                        }
                        override fun onAdClicked() {
                            eventListener?.onAdClicked()
                        }
                        override fun onAdImpression() {
                            eventListener?.onAdImpression()
                        }
                        override fun onAdFailedToShowFullScreenContent(error: AdError) {
                            rewardedAd = null
                            eventListener?.onAdFailedToShow(error.code, error.message)
                        }
                    }
                    loadListener.onAdLoaded()
                }

                override fun onAdFailedToLoad(error: LoadAdError) {
                    rewardedAd = null
                    loadListener.onAdFailedToLoad(error.code, error.message)
                }
            }
        )
    }

    @JvmOverloads
    fun show(
        activity: Activity,
        rewardListener: OnRewardEarnedListener,
        loadListener: OnAdLoadedListener? = null
    ) {
        if (rewardedAd != null) {
            rewardedAd?.show(activity) { rewardItem ->
                rewardListener.onRewardEarned(rewardItem.type, rewardItem.amount)
            }
        } else {
            loadListener?.onAdFailedToLoad(-1, "Ad not loaded yet")
        }
    }

    fun isLoaded(): Boolean = rewardedAd != null
}