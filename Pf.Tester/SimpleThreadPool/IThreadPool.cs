using System.Threading;

namespace Pf.Tester.SimpleThreadPool
{
    /// <summary>
    /// Interface IThreadPool
    /// </summary>
    public interface IThreadPool
    {
        /// <summary>
        ///     Get the current number of threads in the <see cref="IThreadPool" />.
        /// </summary>
        int PoolSize { get; }

        /// <summary>
        ///     Inform the <see cref="IThreadPool" /> of the Scheduler instance's Id,
        ///     prior to initialize being invoked.
        /// </summary>
        string InstanceId { set; }

        /// <summary>
        ///     Inform the <see cref="IThreadPool" /> of the Scheduler instance's name,
        ///     prior to initialize being invoked.
        /// </summary>
        string InstanceName { set; }

        /// <summary>
        ///     Execute the given <see cref="IRunnable" /> in the next
        ///     available <see cref="Thread" />.
        /// </summary>
        /// <remarks>
        ///     The implementation of this interface should not throw exceptions unless
        ///     there is a serious problem (i.e. a serious misconfiguration). If there
        ///     are no available threads, rather it should either queue the Runnable, or
        ///     block until a thread is available, depending on the desired strategy.
        /// </remarks>
        bool RunInThread(IRunnable runnable);

        /// <summary>
        ///     Determines the number of threads that are currently available in in
        ///     the pool.  Useful for determining the number of times
        ///     <see cref="RunInThread(IRunnable)" />  can be called before returning
        ///     false.
        /// </summary>
        /// <remarks>
        ///     The implementation of this method should block until there is at
        ///     least one available thread.
        /// </remarks>
        /// <returns>the number of currently available threads</returns>
        int BlockForAvailableThreads();

        /// <summary>
        ///     Must be called before the <see cref="ThreadPool" /> is
        ///     used, in order to give the it a chance to Initialize.
        /// </summary> 
        void Initialize();

        /// <summary>
        ///     Called by the QuartzScheduler to inform the <see cref="ThreadPool" />
        ///     that it should free up all of it's resources because the scheduler is
        ///     shutting down.
        /// </summary>
        void Shutdown(bool waitForJobsToComplete);
    }
}