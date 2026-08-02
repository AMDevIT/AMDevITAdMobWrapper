package it.amdev.admob.wrapper.utils

import com.google.android.libraries.ads.mobile.sdk.common.FullScreenContentError
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError

internal class ErrorsObjectsExtensions {
    companion object {
        private const val UNKNOWN_ERROR_CODE = -1

        // Common errors like:
        // INTERNAL_ERROR
        // are put int the first decade series.

        internal fun FullScreenContentError.ErrorCode.toInt(): Int {
            val errorCode: Int = when (this) {
                FullScreenContentError.ErrorCode.INTERNAL_ERROR -> 1    // INTERNAL_ERROR should be always 1, because it is a common error for all the ad types.

                // Errors series 300x are for FullScreenContentError.ErrorCode

                FullScreenContentError.ErrorCode.AD_REUSED -> 3001
                FullScreenContentError.ErrorCode.APP_NOT_FOREGROUND -> 3002
                FullScreenContentError.ErrorCode.MEDIATION_SHOW_ERROR -> 3003
                FullScreenContentError.ErrorCode.H5_SHOW_AD_NOT_LOADED -> 3004
                else -> UNKNOWN_ERROR_CODE
            }
            return errorCode
        }

        internal fun LoadAdError.ErrorCode.toInt(): Int {
            val errorCode: Int = when (this) {
                LoadAdError.ErrorCode.INTERNAL_ERROR -> 1

                // Errors series 100x are for LoadAdError.ErrorCode

                LoadAdError.ErrorCode.INVALID_REQUEST -> 1001
                LoadAdError.ErrorCode.NETWORK_ERROR -> 1002
                LoadAdError.ErrorCode.NO_FILL -> 1003
                LoadAdError.ErrorCode.TIMEOUT -> 1004
                LoadAdError.ErrorCode.CANCELLED -> 1005
                LoadAdError.ErrorCode.NOT_FOUND -> 1006
                LoadAdError.ErrorCode.APP_ID_MISSING -> 1007
                LoadAdError.ErrorCode.REQUEST_ID_MISMATCH -> 1008
                LoadAdError.ErrorCode.INVALID_AD_RESPONSE -> 1009
                LoadAdError.ErrorCode.AD_RESPONSE_ALREADY_USED -> 1010
                else -> UNKNOWN_ERROR_CODE
            }
            return errorCode
        }
    }
}