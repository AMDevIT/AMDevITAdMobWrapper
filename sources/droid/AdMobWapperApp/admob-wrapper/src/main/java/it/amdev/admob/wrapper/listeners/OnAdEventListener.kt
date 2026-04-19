package it.amdev.admob.wrapper.listeners

interface OnAdEventListener {
    fun onAdShown()
    fun onAdDismissed()
    fun onAdClicked()
    fun onAdImpression()
    fun onAdFailedToShow(errorCode: Int, errorMessage: String)
}