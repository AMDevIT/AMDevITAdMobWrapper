package it.amdev.admob.wrapper.diagnostics

interface IDroidLogger {
    fun isEnabled(level: LogLevel): Boolean

    fun logTrace(message: String, tag: String? = null)
    fun logDebug(message: String, tag: String? = null)
    fun logInfo(message: String, tag: String? = null)
    fun logWarning(message: String, tag: String? = null)
    fun logError(message: String, tag: String? = null)
    fun logCritical(message: String, tag: String? = null)
}