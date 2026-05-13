using Microsoft.Extensions.Logging;

namespace AMDevIT.Admob.Wrapper.MAUITestApp.Services
{
    /// <summary>
    /// Provides agnostic approach for enqueuing actions to the main thread dispatcher, if available. If no dispatcher is available for the current thread, it tries to execute the action synchronously and logs a warning.
    /// </summary>
    /// <param name="logger">The logger instance used for logging warnings and errors.</param>
    /// <remarks>This class is used to not bind the viewmodels to a MAUI-only implementation, allowing for easier testing and separation of concerns.</remarks>
    public partial class DispatcherService(ILogger<DispatcherService> logger) 
        : IDispatcherService
    {
        #region Properties

        protected ILogger<DispatcherService> Logger => logger;

        #endregion

        #region Methods

        public async Task TryEnqueue(Action action)
        {
            IDispatcher? dispatcher = Dispatcher.GetForCurrentThread();

            if (dispatcher is null)
            {
                if (this.Logger.IsEnabled(LogLevel.Warning))
                    this.Logger.LogWarning("No dispatcher available for the current thread. Trying executing action synchronously.");
                try
                {
                    action();
                }
                catch (Exception exc)
                {
                    if (this.Logger.IsEnabled(LogLevel.Error))
                        this.Logger.LogError(exc, "Cannot execute action synchronously, no dispatcher available.");
                }
                return;
            }

            if (dispatcher.IsDispatchRequired)
            {
                await dispatcher.DispatchAsync(action);
            }
            else
            {
                action();
            }
        }

        public async Task TryEnqueue(Func<Task> executionTask)
        {
            IDispatcher? dispatcher = Dispatcher.GetForCurrentThread();

            if (dispatcher is null)
            {
                if (this.Logger.IsEnabled(LogLevel.Warning))
                    this.Logger.LogWarning("No dispatcher available for the current thread. Trying executing action synchronously.");
                try
                {
                    await executionTask();
                }
                catch (Exception exc)
                {
                    if (this.Logger.IsEnabled(LogLevel.Error))
                        this.Logger.LogError(exc, "Cannot execute action synchronously, no dispatcher available.");
                }
                return;
            }

            if (dispatcher.IsDispatchRequired)
            {
                await dispatcher.DispatchAsync(executionTask);
            }
            else
            {
                await executionTask();
            }
        }

        #endregion
    }
}
