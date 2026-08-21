package it.amdev.admob.wrapper.ads

internal class CallbackLifecycle {

    private val syncRoot = Any()
    private var generation = 0L
    private var destroyed = false

    fun <T> begin(action: (Long) -> T): T {
        synchronized(this.syncRoot) {
            check(!this.destroyed) { "The ad wrapper has been destroyed" }
            this.generation++
            return action(this.generation)
        }
    }

    fun <T> access(callbackGeneration: Long, action: () -> T): T? {
        synchronized(this.syncRoot) {
            if (this.destroyed || callbackGeneration != this.generation)
                return null

            return action()
        }
    }

    fun destroy(action: () -> Unit): Boolean {
        synchronized(this.syncRoot) {
            if (this.destroyed)
                return false

            this.destroyed = true
            this.generation++
            action()
            return true
        }
    }

    fun <T> read(action: () -> T): T {
        synchronized(this.syncRoot) {
            return action()
        }
    }
}
