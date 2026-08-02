package it.amdev.admob.wrapper.privacy

class ConsentInformationRequestDebugParameters(val debugGeography: Int? = null,
                                               val  testDeviceHashedId: String? = null) {
    override fun toString(): String {
        return "ConsentInformationRequestDebugParameters(debugGeography=$debugGeography, testDeviceHashedId=$testDeviceHashedId)"
    }
}