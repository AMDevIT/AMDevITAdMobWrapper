package it.amdev.admob.wrapper.ads

import android.app.Activity
import com.google.android.libraries.ads.mobile.sdk.common.AdLoadCallback
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError
import com.google.android.libraries.ads.mobile.sdk.common.AdRequest
import com.google.android.libraries.ads.mobile.sdk.common.AdValue
import com.google.android.libraries.ads.mobile.sdk.common.FullScreenContentError
import com.google.android.libraries.ads.mobile.sdk.rewarded.RewardedAd
import com.google.android.libraries.ads.mobile.sdk.rewarded.RewardedAdEventCallback
import it.amdev.admob.wrapper.diagnostics.IDroidLogger
import it.amdev.admob.wrapper.listeners.OnAdEventListener
import it.amdev.admob.wrapper.listeners.OnAdLoadedListener
import it.amdev.admob.wrapper.listeners.OnRewardEarnedListener
import it.amdev.admob.wrapper.utils.ErrorsObjectsExtensions.Companion.toInt

@Suppress("unused")
class RewardedAdWrapper(private val logger: IDroidLogger? = null) {

    private var rewardedAd: RewardedAd? = null

    @JvmOverloads
    fun load(adUnitId: String,
             loadListener: OnAdLoadedListener,
             eventListener: OnAdEventListener? = null)
    {
        val adRequest = AdRequest.Builder(adUnitId =  adUnitId)
                                 .build()

        RewardedAd.load(adRequest,
            object : AdLoadCallback<RewardedAd> {
                override fun onAdLoaded(ad: RewardedAd) {
                    rewardedAd = ad
                    rewardedAd?.adEventCallback = object : RewardedAdEventCallback {
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

                        override fun onAdPaid(value: AdValue) {
                            super.onAdPaid(value)
                            logger?.logDebug(tag = LOG_TAG,
                                             message = "Ad paid: ${value.valueMicros} ${value.currencyCode}")
                        }

                        override fun onAdMetadataChanged() {
                            super.onAdMetadataChanged()
                            logger?.logDebug(tag = LOG_TAG,
                                             message = "Ad metadata changed")
                        }

                        override fun onAdFailedToShowFullScreenContent(fullScreenContentError: FullScreenContentError) {
                            rewardedAd = null

                            val errorCode = fullScreenContentError.code.toInt()
                            eventListener?.onAdFailedToShow(errorCode = errorCode,
                                                            errorMessage = fullScreenContentError.message)
                        }
                    }

                    loadListener.onAdLoaded()
                }

                override fun onAdFailedToLoad(adError: LoadAdError) {
                    rewardedAd = null

                    val errorCode = adError.code.toInt()
                    loadListener.onAdFailedToLoad(errorCode = errorCode,
                                                  errorMessage = adError.message)
                }
            }
        )
    }

    @JvmOverloads
    fun show(activity: Activity,
             rewardListener: OnRewardEarnedListener,
             loadListener: OnAdLoadedListener? = null) {
        if (rewardedAd != null) {
            rewardedAd?.show(activity) { rewardItem ->
                rewardListener.onRewardEarned(rewardItem.type, rewardItem.amount)
            }
        } else {
            loadListener?.onAdFailedToLoad(-1, "Ad not loaded yet")
        }
    }

    fun isLoaded(): Boolean = rewardedAd != null

    companion object {
        private const val LOG_TAG = "RewardedAdWrapper"
    }
}