using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace PerformanceTool.SimpleThreadPool
{
    /// <summary>
    /// 简单线程池
    /// 
    /// </summary>
    /// <remarks>
    /// RunInThread(IRunnable runnable) 增加try catch 确保handoffPending在添加任务完成后设置为false，防止停止线程池时，无法停止。一直等待handoffPending变为true的问题。
    /// </remarks>
    public class SimpleThreadPool : IThreadPool
    {
        //private const int DefaultThreadPoolSize = 10;
        private static readonly SimpleLogger.ILog log = SimpleLogger.LogManager.GetLogger(typeof(SimpleThreadPool));

        private readonly LinkedList<WorkerThread> availWorkers = new LinkedList<WorkerThread>();
        private readonly LinkedList<WorkerThread> busyWorkers = new LinkedList<WorkerThread>();
        private readonly object nextRunnableLock = new object();
        private readonly string schedulerInstanceName = null;

        private int count = Environment.ProcessorCount * 2;
        private volatile bool handoffPending;
        private volatile bool isShutdown;
        private ThreadPriority prio = ThreadPriority.Normal;

        private List<WorkerThread> workers;
        private bool _makeThreadsDaemons = true;

        /// <summary>
        ///     Create a new (unconfigured) <see cref="SimpleThreadPool" />.
        /// </summary>
        public SimpleThreadPool()
        {
        }

        /// <summary>
        ///     Create a new <see cref="SimpleThreadPool" /> with the specified number
        ///     of <see cref="Thread" /> s that have the given priority.
        /// </summary>
        /// <param name="threadCount">
        ///     the number of worker <see cref="Thread" />s in the pool, must
        ///     be > 0.
        /// </param>
        /// <param name="threadPriority">
        ///     the thread priority for the worker threads.
        /// </param>
        public SimpleThreadPool(int threadCount, ThreadPriority threadPriority)
        {
            ThreadCount = threadCount;
            ThreadPriority = threadPriority;
        }

        /// <summary>
        ///     Gets or sets the number of worker threads in the pool.
        ///     Set  has no effect after <see cref="Initialize()" /> has been called.
        /// </summary>
        public int ThreadCount
        {
            get { return count; }
            set { count = value; }
        }

        /// <summary>
        ///     Get or set the thread priority of worker threads in the pool.
        ///     Set operation has no effect after <see cref="Initialize()" /> has been called.
        /// </summary>
        public ThreadPriority ThreadPriority
        {
            get { return prio; }
            set { prio = value; }
        }

        /// <summary>
        ///     Gets or sets the thread name prefix.
        /// </summary>
        /// <value>The thread name prefix.</value>
        public virtual string ThreadNamePrefix { get; set; }

        /// <summary>
        ///     Gets or sets the value of makeThreadsDaemons.
        /// </summary>
        public virtual bool MakeThreadsDaemons
        {
            get { return _makeThreadsDaemons; }
            set { _makeThreadsDaemons = value; }
        }

        /// <summary>
        ///     Gets the size of the pool.
        /// </summary>
        /// <value>The size of the pool.</value>
        public virtual int PoolSize
        {
            get { return ThreadCount; }
        }

        /// <summary>
        ///     Inform the <see cref="IThreadPool" /> of the Scheduler instance's Id,
        ///     prior to initialize being invoked.
        /// </summary>
        public virtual string InstanceId
        {
            set { }
        }

        /// <summary>
        ///     Inform the <see cref="IThreadPool" /> of the Scheduler instance's name,
        ///     prior to initialize being invoked.
        /// </summary>
        public virtual string InstanceName
        {
            set { }
        }

        /// <summary>
        ///     Called by the QuartzScheduler before the <see cref="ThreadPool" /> is
        ///     used, in order to give the it a chance to Initialize.
        /// </summary>
        public virtual void Initialize()
        {
            if (workers != null && workers.Count > 0)
            {
                // already initialized...
                return;
            }

            if (count <= 0)
            {
                throw new Exception("Thread count must be > 0");
            }

            // create the worker threads and start them
            foreach (WorkerThread wt in CreateWorkerThreads(count))
            {
                wt.Start();
                availWorkers.AddLast(wt);
            }
        }

        /// <summary>
        ///     Terminate any worker threads in this thread group.
        ///     Jobs currently in progress will complete.
        /// </summary>
        public virtual void Shutdown(bool waitForJobsToComplete)
        {
            // Give waiting (wait(1000)) worker threads a chance to shut down.
            // Active worker threads will shut down after finishing their
            // current job.
            lock (nextRunnableLock)
            {
                log.Debug("Shutting down threadpool...");
                isShutdown = true;

                if (workers == null) // case where the pool wasn't even initialize()ed
                {
                    return;
                }

                // signal each worker thread to shut down
                foreach (WorkerThread thread in workers)
                {
                    if (thread != null)
                    {
                        thread.Shutdown();
                    }
                }
                Monitor.PulseAll(nextRunnableLock);


                if (waitForJobsToComplete)
                {
                    bool interrupted = false;
                    try
                    {
                        // wait for hand-off in runInThread to complete...
                        while (handoffPending)
                        {
                            try
                            {
                                Monitor.Wait(nextRunnableLock, 100);
                            }
                            catch (ThreadInterruptedException)
                            {
                                interrupted = true;
                            }
                        }

                        // Wait until all worker threads are shut down
                        while (busyWorkers.Count > 0)
                        {
                            LinkedListNode<WorkerThread> wt = busyWorkers.First;
                            try
                            {
                                log.DebugFormat(CultureInfo.InvariantCulture, "Waiting for thread {0} to shut down",
                                                wt.Value.Name);

                                // note: with waiting infinite time the
                                // application may appear to 'hang'.
                                Monitor.Wait(nextRunnableLock, 2000);
                            }
                            catch (ThreadInterruptedException)
                            {
                                interrupted = true;
                            }
                        }

                        while (workers.Count > 0)
                        {
                            int index = workers.Count - 1;
                            WorkerThread wt = workers[index];
                            try
                            {
                                wt.Join();
                                workers.RemoveAt(index);
                            }
                            catch (ThreadInterruptedException)
                            {
                                interrupted = true;
                            }
                        }
                    }
                    finally
                    {
                        if (interrupted)
                        {
                            Thread.CurrentThread.Interrupt();
                        }
                    }

                    log.Debug("No executing jobs remaining, all threads stopped.");
                }

                log.Debug("Shutdown of threadpool complete.");
            }
        }

        /// <summary>
        ///     Run the given <see cref="IRunnable" /> object in the next available
        ///     <see cref="Thread" />. If while waiting the thread pool is asked to
        ///     shut down, the Runnable is executed immediately within a new additional
        ///     thread.
        /// </summary>
        /// <param name="runnable">
        ///     The <see cref="IRunnable" /> to be added.
        /// </param>
        public virtual bool RunInThread(IRunnable runnable)
        {
            if (runnable == null)
            {
                return false;
            }

            lock (nextRunnableLock)
            {
                try
                {
                    handoffPending = true;

                    // Wait until a worker thread is available
                    while ((availWorkers.Count < 1) && !isShutdown)
                    {
                        try
                        {
                            Monitor.Wait(nextRunnableLock, 500);
                        }
                        catch (ThreadInterruptedException)
                        {
                        }
                    }

                    if (!isShutdown)
                    {
                        WorkerThread wt = availWorkers.First.Value;
                        availWorkers.RemoveFirst();
                        busyWorkers.AddLast(wt);
                        wt.Run(runnable);
                    }
                    else
                    {
                        //停止后不再添加job
                        // If the thread pool is going down, execute the Runnable
                        // within a new additional worker thread (no thread from the pool).
                        //var wt = new WorkerThread(this, "WorkerThread-LastJob", prio, MakeThreadsDaemons, runnable);
                        //busyWorkers.AddLast(wt);
                        //workers.Add(wt);
                        //wt.Start();
                    }
                    Monitor.PulseAll(nextRunnableLock);
                }
                catch (Exception exDebug)
                {
                    log.Fatal("添加任务时异常！！", exDebug);
                }
                finally
                {
                    handoffPending = false;
                }
            }

            return !isShutdown;
        }

        /// <summary>
        /// Determines the number of threads that are currently available in in
        /// the pool.  Useful for determining the number of times
        /// <see cref="RunInThread(IRunnable)" />  can be called before returning
        /// false.
        /// </summary>
        /// <returns>the number of currently available threads</returns>
        /// <remarks>The implementation of this method should block until there is at
        /// least one available thread.</remarks>
        public int BlockForAvailableThreads()
        {
            lock (nextRunnableLock)
            {
                while ((availWorkers.Count < 1 || handoffPending) && !isShutdown)
                {
                    try
                    {
                        Monitor.Wait(nextRunnableLock, 500);
                    }
                    catch (ThreadInterruptedException)
                    {
                    }
                }

                return availWorkers.Count;
            }
        }

        /// <summary>
        /// Makes the available.
        /// </summary>
        /// <param name="wt">The wt.</param>
        protected void MakeAvailable(WorkerThread wt)
        {
            lock (nextRunnableLock)
            {
                if (!isShutdown)
                {
                    availWorkers.AddLast(wt);
                }
                busyWorkers.Remove(wt);
                Monitor.PulseAll(nextRunnableLock);
            }
        }

        /// <summary>
        ///     Creates the worker threads.
        /// </summary>
        /// <param name="threadCount">The thread count.</param>
        /// <returns></returns>
        protected virtual IList<WorkerThread> CreateWorkerThreads(int threadCount)
        {
            workers = new List<WorkerThread>();
            for (int i = 1; i <= threadCount; ++i)
            {
                string threadPrefix = ThreadNamePrefix;
                if (threadPrefix == null)
                {
                    threadPrefix = schedulerInstanceName + "_Worker";
                }

                var workerThread = new WorkerThread(
                    this,
                    string.Format(CultureInfo.InvariantCulture, "{0}-{1}", threadPrefix, i),
                    ThreadPriority,
                    MakeThreadsDaemons);

                workers.Add(workerThread);
            }

            return workers;
        }

        /// <summary>
        ///     Terminate any worker threads in this thread group.
        ///     Jobs currently in progress will complete.
        /// </summary>
        public virtual void Shutdown()
        {
            Shutdown(true);
        }


        /// <summary>
        /// Clears from busy workers list.
        /// </summary>
        /// <param name="wt">The wt.</param>
        protected virtual void ClearFromBusyWorkersList(WorkerThread wt)
        {
            lock (nextRunnableLock)
            {
                busyWorkers.Remove(wt);
                Monitor.PulseAll(nextRunnableLock);
            }
        }

        /// <summary>
        ///     A Worker loops, waiting to Execute tasks.
        /// </summary>
        protected class WorkerThread : TSharpThread
        {
            private readonly object lockObject = new object();
            private readonly bool runOnce;
            private readonly SimpleThreadPool tp;

            // A flag that signals the WorkerThread to terminate.
            private volatile bool run = true;

            private IRunnable runnable;

            /// <summary>
            ///     Create a worker thread and start it. Waiting for the next Runnable,
            ///     executing it, and waiting for the next Runnable, until the Shutdown
            ///     flag is set.
            /// </summary>
            internal WorkerThread(SimpleThreadPool tp, string name,
                                  ThreadPriority prio, bool isDaemon)
                : this(tp, name, prio, isDaemon, null)
            {
            }

            /// <summary>
            ///     Create a worker thread, start it, Execute the runnable and terminate
            ///     the thread (one time execution).
            /// </summary>
            internal WorkerThread(SimpleThreadPool tp, string name,
                                  ThreadPriority prio, bool isDaemon, IRunnable runnable)
                : base(name)
            {
                this.tp = tp;
                this.runnable = runnable;
                if (runnable != null)
                {
                    runOnce = true;
                }
                Priority = prio;
                IsBackground = isDaemon;
            }

            /// <summary>
            ///     Signal the thread that it should terminate.
            /// </summary>
            internal virtual void Shutdown()
            {
                run = false;
            }


            /// <summary>
            /// Runs the specified new runnable.
            /// </summary>
            /// <param name="newRunnable">The new runnable.</param>
            /// <exception cref="System.ArgumentException">Already running a Runnable!</exception>
            public void Run(IRunnable newRunnable)
            {
                lock (lockObject)
                {
                    if (runnable != null)
                    {
                        throw new ArgumentException("Already running a Runnable!");
                    }

                    runnable = newRunnable;
                    Monitor.PulseAll(lockObject);
                }
            }

            /// <summary>
            ///     Loop, executing targets as they are received.
            /// </summary>
            public override void Run()
            {
                bool ran = false;

                while (run)
                {
                    try
                    {
                        lock (lockObject)
                        {
                            while (runnable == null && run)
                            {
                                Monitor.Wait(lockObject, 500);
                            }
                            if (runnable != null)
                            {
                                ran = true;
                                runnable.Run();
                            }
                        }
                    }
                    catch (Exception exceptionInRunnable)
                    {
                        log.Error("Error while executing the Runnable: ", exceptionInRunnable);
                    }
                    finally
                    {
                        lock (lockObject)
                        {
                            try
                            {
                                var tmp = runnable as IRunnerDisposable;
                                if (tmp != null)
                                    tmp.Dispose();
                            }
                            catch (Exception disposeException)
                            {
                                log.Error("Error while dispose the Runnable: ", disposeException);
                            }
                            finally
                            {
                                runnable = null;
                            }
                        }
                        // repair the thread in case the runnable mucked it up...
                        Priority = tp.ThreadPriority;

                        if (runOnce)
                        {
                            run = false;
                            tp.ClearFromBusyWorkersList(this);
                        }
                        else if (ran)
                        {
                            ran = false;
                            tp.MakeAvailable(this);
                        }
                    }
                }

                log.Debug("WorkerThread is shut down");
            }
        }

        /// <summary>
        /// Queues the work item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <param name="item">The item.</param>
        public bool QueueWorkItem<T>(Action<T> action, T item)
        {
            return RunInThread(new NestRunner<T>(action, item));
        }

        class NestRunner<T> : IRunnable
        {
            private readonly T t;
            private readonly Action<T> action;

            public NestRunner(Action<T> action, T t)
            {
                this.t = t;
                this.action = action;
            }

            public void Run()
            {
                action(t);
            }
        }

    }
}