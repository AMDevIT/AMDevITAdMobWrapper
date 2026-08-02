package it.amdev.admob.wrapper.listeners

interface OnConsentGatheringListener {
    fun onCompleted(canRequestAds: Boolean,
                    privacyOptionsRequired: Boolean)

    fun onCompletedWithError(errorCode: Int,
                             errorMessage: String,
                             canRequestAds: Boolean,
                             privacyOptionsRequired: Boolean)
}