package it.amdev.admob.wrapper.listeners

interface OnAdLoadedListener {
    fun onAdLoaded()
    fun onAdFailedToLoad(errorCode: Int, errorMessage: String)
}