package it.amdev.admob.wrapper.ads

import android.app.Activity
import com.google.android.libraries.ads.mobile.sdk.common.AdLoadCallback
import com.google.android.libraries.ads.mobile.sdk.common.AdRequest
import com.google.android.libraries.ads.mobile.sdk.common.AdValue
import com.google.android.libraries.ads.mobile.sdk.common.FullScreenContentError
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError
import com.google.android.libraries.ads.mobile.sdk.rewarded.RewardedAd
import com.google.android.libraries.ads.mobile.sdk.rewarded.RewardedAdEventCallback
import it.amdev.admob.wrapper.diagnostics.IDroidLogger
import it.amdev.admob.wrapper.listeners.OnAdEventListener
import it.amdev.admob.wrapper.listeners.OnAdLoadedListener
import it.amdev.admob.wrapper.listeners.OnRewardEarnedListener
import it.amdev.admob.wrapper.utils.ErrorsObjectsExtensions.Companion.toInt

@Suppress("unused")
class RewardedAdWrapper(logger: IDroidLogger? = null) {

    private val lifecycle = CallbackLifecycle()
    private var generation = 0L
    private var rewardedAd: RewardedAd? = null
    private var loadListener: OnAdLoadedListener? = null
    private var eventListener: OnAdEventListener? = null
    private var rewardListener: OnRewardEarnedListener? = null
    private var logger: IDroidLogger? = logger

    @JvmOverloads
    fun load(adUnitId: String,
             loadListener: OnAdLoadedListener,
             eventListener: OnAdEventListener? = null) {
        this.lifecycle.begin { generation ->
            this.generation = generation
            this.clearAdLocked()
            this.loadListener = loadListener
            this.eventListener = eventListener
            this.rewardListener = null

            val adRequest = AdRequest.Builder(adUnitId = adUnitId).build()
            RewardedAd.load(adRequest,
                            object : AdLoadCallback<RewardedAd> {
                                override fun onAdLoaded(ad: RewardedAd) {
                                    val accepted = lifecycle.access(generation) {
                                        rewardedAd = ad
                                        ad.adEventCallback = createEventCallback(generation)
                                        true
                                    } == true

                                    if (!accepted) {
                                        clearCallback(ad)
                                        return
                                    }

                                    notifyLoaded(generation)
                                }

                                override fun onAdFailedToLoad(adError: LoadAdError) {
                                    lifecycle.access(generation) { rewardedAd = null }
                                    notifyLoadFailed(generation,
                                                     adError.code.toInt(),
                                                     adError.message)
                                }
                            })
        }
    }

    @JvmOverloads
    fun show(activity: Activity,
             rewardListener: OnRewardEarnedListener,
             loadListener: OnAdLoadedListener? = null) {
        this.lifecycle.access(this.generation) {
            val ad = this.rewardedAd
            if (ad != null) {
                this.rewardListener = rewardListener
                val showGeneration = this.generation
                ad.show(activity) { rewardItem ->
                    this.notifyReward(showGeneration, rewardItem.type, rewardItem.amount)
                }
            } else {
                loadListener?.let { listener ->
                    this.invokeSafely { listener.onAdFailedToLoad(-1, "Ad not loaded yet") }
                }
            }
        }
    }

    fun isLoaded(): Boolean {
        return this.lifecycle.access(this.generation) { this.rewardedAd != null } == true
    }

    fun destroy() {
        this.lifecycle.destroy {
            this.loadListener = null
            this.eventListener = null
            this.rewardListener = null
            this.logger = null
            this.clearAdLocked()
        }
    }

    private fun createEventCallback(generation: Long): RewardedAdEventCallback {
        return object : RewardedAdEventCallback {
            override fun onAdShowedFullScreenContent() {
                this@RewardedAdWrapper.notifyEvent(generation) { it.onAdShown() }
            }

            override fun onAdDismissedFullScreenContent() {
                this@RewardedAdWrapper.lifecycle.access(generation) {
                    rewardedAd = null
                    rewardListener = null
                }
                this@RewardedAdWrapper.notifyEvent(generation) { it.onAdDismissed() }
            }

            override fun onAdClicked() {
                this@RewardedAdWrapper.notifyEvent(generation) { it.onAdClicked() }
            }

            override fun onAdImpression() {
                this@RewardedAdWrapper.notifyEvent(generation) { it.onAdImpression() }
            }

            override fun onAdPaid(value: AdValue) {
                super.onAdPaid(value)
                this@RewardedAdWrapper.logDebug(generation,
                                                "Ad paid: ${value.valueMicros} ${value.currencyCode}")
            }

            override fun onAdMetadataChanged() {
                super.onAdMetadataChanged()
                this@RewardedAdWrapper.logDebug(generation, "Ad metadata changed")
            }

            override fun onAdFailedToShowFullScreenContent(fullScreenContentError: FullScreenContentError) {
                this@RewardedAdWrapper.lifecycle.access(generation) {
                    rewardedAd = null
                    rewardListener = null
                }
                this@RewardedAdWrapper.notifyEvent(generation) {
                    it.onAdFailedToShow(errorCode = fullScreenContentError.code.toInt(),
                                        errorMessage = fullScreenContentError.message)
                }
            }
        }
    }

    private fun clearAdLocked() {
        this.rewardedAd?.let { this.clearCallback(it) }
        this.rewardedAd = null
    }

    private fun clearCallback(ad: RewardedAd) {
        try {
            ad.adEventCallback = NO_OP_EVENT_CALLBACK
        } catch (_: Throwable) {
            // Native callback cleanup must never make teardown fail.
        }
    }

    private fun notifyLoaded(generation: Long) {
        this.lifecycle.access(generation) {
            this.loadListener?.let { listener -> this.invokeSafely { listener.onAdLoaded() } }
        }
    }

    private fun notifyLoadFailed(generation: Long, errorCode: Int, errorMessage: String) {
        this.lifecycle.access(generation) {
            this.loadListener?.let { listener ->
                this.invokeSafely {
                    listener.onAdFailedToLoad(errorCode = errorCode,
                                              errorMessage = errorMessage)
                }
            }
        }
    }

    private fun notifyEvent(generation: Long, callback: (OnAdEventListener) -> Unit) {
        this.lifecycle.access(generation) {
            this.eventListener?.let { listener -> this.invokeSafely { callback(listener) } }
        }
    }

    private fun notifyReward(generation: Long, type: String, amount: Int) {
        this.lifecycle.access(generation) {
            this.rewardListener?.let { listener ->
                this.invokeSafely { listener.onRewardEarned(type, amount) }
            }
        }
    }

    private fun logDebug(generation: Long, message: String) {
        this.lifecycle.access(generation) {
            this.logger?.let { activeLogger ->
                this.invokeSafely { activeLogger.logDebug(tag = LOG_TAG, message = message) }
            }
        }
    }

    private fun invokeSafely(callback: () -> Unit) {
        try {
            callback()
        } catch (_: Throwable) {
            // Managed diagnostics and listeners must never crash a native callback thread.
        }
    }

    companion object {
        private const val LOG_TAG = "RewardedAdWrapper"
        private val NO_OP_EVENT_CALLBACK = object : RewardedAdEventCallback {}
    }
}
