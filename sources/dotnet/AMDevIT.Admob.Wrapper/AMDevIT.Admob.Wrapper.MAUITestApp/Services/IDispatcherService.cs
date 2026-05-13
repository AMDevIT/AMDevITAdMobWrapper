namespace AMDevIT.Admob.Wrapper.MAUITestApp.Services
{
    /// <summary>
    /// Provides the interface methods for an agnostic approach for enqueuing actions to a dispatcher. The behavior of the behaviour of the implementation could vary.
    /// </summary>
    public interface IDispatcherService
    {
        #region Methods

        Task TryEnqueue(Action action);
        Task TryEnqueue(Func<Task> executionTask);

        #endregion
    }
}