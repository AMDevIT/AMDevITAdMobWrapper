package it.amdev.admob.wrapper.listeners

interface OnConsentInformationRequestListener {
    fun onConsentInformationRequestSuccess()
    fun onConsentInformationRequestFailure(errorCode: Int, errorMessage: String)
}