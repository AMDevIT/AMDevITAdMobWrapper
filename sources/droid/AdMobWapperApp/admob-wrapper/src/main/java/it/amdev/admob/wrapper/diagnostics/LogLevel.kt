package it.amdev.admob.wrapper.diagnostics

// Represents the severity level of a log message.
// It's based on the Microsoft.Extensions.Logging.LogLevel enumeration, which is commonly used in .NET applications for logging purposes.
// This equivalence allows cross platform developers to create dotNet wrappers for Android application with similar logging behavior and severity levels.
enum class LogLevel(private val value: Int) {
    // Logs that contain the most detailed messages. These messages may contain sensitive application data.
    // These messages are disabled by default and should never be enabled in a production environment.
    Trace(value = 0),

    // Logs that are used for interactive investigation during development.  These logs should primarily contain
    // information useful for debugging and have no long-term value.
    Debug(value = 1),

    // Logs that track the general flow of the application. These logs should have long-term value.
    Information(value = 2),

    // Logs that highlight an abnormal or unexpected event in the application flow, but do not otherwise cause the
    // application execution to stop.
    Warning(value = 3),

    // Logs that highlight when the current flow of execution is stopped due to a failure. These should indicate a
    // failure in the current activity, not an application-wide failure.
    Error(value = 4),

    // Logs that describe an unrecoverable application or system crash, or a catastrophic failure that requires
    // immediate attention.
    Critical(value = 5),

    // Not used for writing log messages. Specifies that a logging category should not write any messages.
    None(value = 6);

    companion object {
        fun fromInt(value: Int): LogLevel {
            return when (value) {
                0 -> Trace
                1 -> Debug
                2 -> Information
                3 -> Warning
                4 -> Error
                5 -> Critical
                6 -> None
                else -> throw IllegalArgumentException("Invalid log level value: $value")
            }
        }

        fun toInt(logLevel: LogLevel): Int {
            return logLevel.value
        }
    }
}