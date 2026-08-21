package it.amdev.admob.wrapper.ads

import org.junit.Assert.assertEquals
import org.junit.Assert.assertFalse
import org.junit.Assert.assertNotNull
import org.junit.Assert.assertNull
import org.junit.Assert.assertTrue
import org.junit.Test
import java.lang.ref.WeakReference
import java.util.concurrent.CountDownLatch
import java.util.concurrent.TimeUnit
import java.util.concurrent.atomic.AtomicInteger

class CallbackLifecycleTest {

    @Test
    fun callbackAfterDestroyIsIgnored() {
        val lifecycle = CallbackLifecycle()
        val generation = lifecycle.begin { it }

        lifecycle.destroy {}

        assertNull(lifecycle.access(generation) { "callback" })
    }

    @Test
    fun destroyIsIdempotent() {
        val lifecycle = CallbackLifecycle()
        val teardownCount = AtomicInteger()

        assertTrue(lifecycle.destroy { teardownCount.incrementAndGet() })
        assertFalse(lifecycle.destroy { teardownCount.incrementAndGet() })
        assertEquals(1, teardownCount.get())
    }

    @Test
    fun reloadInvalidatesPreviousGeneration() {
        val lifecycle = CallbackLifecycle()
        val firstGeneration = lifecycle.begin { it }
        val secondGeneration = lifecycle.begin { it }

        assertNull(lifecycle.access(firstGeneration) { "stale" })
        assertEquals("current", lifecycle.access(secondGeneration) { "current" })
    }

    @Test
    fun destroyWaitsForConcurrentCallbackAndBlocksLaterCallbacks() {
        val lifecycle = CallbackLifecycle()
        val generation = lifecycle.begin { it }
        val callbackEntered = CountDownLatch(1)
        val releaseCallback = CountDownLatch(1)
        val destroyCompleted = CountDownLatch(1)
        val callbackCount = AtomicInteger()

        val callbackThread = Thread {
            lifecycle.access(generation) {
                callbackEntered.countDown()
                assertTrue(releaseCallback.await(5, TimeUnit.SECONDS))
                callbackCount.incrementAndGet()
            }
        }
        callbackThread.start()
        assertTrue(callbackEntered.await(5, TimeUnit.SECONDS))

        val destroyThread = Thread {
            lifecycle.destroy {}
            destroyCompleted.countDown()
        }
        destroyThread.start()

        assertFalse(destroyCompleted.await(100, TimeUnit.MILLISECONDS))
        releaseCallback.countDown()
        callbackThread.join(5_000)
        destroyThread.join(5_000)

        assertEquals(1, callbackCount.get())
        assertNull(lifecycle.access(generation) { callbackCount.incrementAndGet() })
        assertEquals(1, callbackCount.get())
    }

    @Test
    fun managedReferenceRemainsAliveUntilTeardown() {
        val lifecycle = CallbackLifecycle()
        var listener: Any? = Any()
        val reference = WeakReference(listener)
        val generation = lifecycle.begin { it }

        repeat(3) {
            System.gc()
            Thread.yield()
        }

        assertNotNull(reference.get())
        assertNotNull(lifecycle.access(generation) { listener })

        lifecycle.destroy { listener = null }
        assertNull(lifecycle.access(generation) { listener })
    }

    @Test
    fun disposedManagedTargetIsNeverInvoked() {
        val lifecycle = CallbackLifecycle()
        val callbackCount = AtomicInteger()
        var managedTarget: (() -> Unit)? = { callbackCount.incrementAndGet() }
        val generation = lifecycle.begin { it }

        lifecycle.destroy { managedTarget = null }
        lifecycle.access(generation) { managedTarget?.invoke() }

        assertEquals(0, callbackCount.get())
    }
}
