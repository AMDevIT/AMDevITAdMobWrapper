package it.amdev.admob.wrapper.listeners

interface OnInitializedListener {
    fun onInitialized()
    fun onInitializationFailed(error: String)
}