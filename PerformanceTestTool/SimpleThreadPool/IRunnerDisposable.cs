using System;

namespace PerformanceTool.SimpleThreadPool
{
    /// <summary>
    /// This interface should be implemented by any class whose instances are intended
    /// to be executed by a thread. and auto dispose after run
    /// </summary>
    public interface IRunnerDisposable : IRunnable, IDisposable
    {

    }
}