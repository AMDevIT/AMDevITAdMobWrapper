package it.amdev.admob.wrapper.privacy

class ConsentStatusData(val lastRefreshTimestampMilliseconds: Long,
                        val consentStatus:Int,
                        val privacyOptionsRequirementStatus: Int) {
    override fun toString(): String {
        return "ConsentStatusData(lastRefreshTimestamp=$lastRefreshTimestampMilliseconds, consentStatus=$consentStatus, " +
               "privacyOptionsRequirementStatus=$privacyOptionsRequirementStatus)"
    }
}