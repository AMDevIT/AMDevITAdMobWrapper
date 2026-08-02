package it.amdev.admob.wrapper.listeners

interface OnConsentFormEventListener {
    fun onDismissed()
    fun onDismissedWithError(errorCode: Int, errorMessage: String?)
}