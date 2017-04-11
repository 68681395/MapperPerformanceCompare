using System.Globalization;
using System.Threading;

namespace Pf.Tester.SimpleThreadPool
{
    /// <summary>
    ///线程基类
    /// </summary>
    public abstract class TSharpThread : IRunnable
    {
        /// <summary>
        ///     The instance of System.Threading.Thread
        /// </summary>
        private readonly Thread thread;


        /// <summary>
        /// Initializes a new instance of the <see cref="TSharpThread" /> class.
        /// </summary>
        protected TSharpThread()
        { 
            thread = new Thread(Run);
        }
        /// <summary>
        ///     Initializes a new instance of the Thread class.
        /// </summary>
        /// <param name="name">The name of the thread</param>
        protected TSharpThread(string name)
        {
            thread = new Thread(Run);
            Name = name;
        }

        /// <summary>
        ///     Gets or sets the name of the thread
        /// </summary>
        public string Name
        {
            get { return thread.Name; }
            protected set { thread.Name = value; }
        }

        /// <summary>
        ///     Gets or sets a value indicating the scheduling priority of a thread
        /// </summary>
        protected ThreadPriority Priority
        {
            get { return thread.Priority; }
            set { thread.Priority = value; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether or not a thread is a background thread.
        /// </summary>
        protected bool IsBackground
        {
            set { thread.IsBackground = value; }
        }

        /// <summary>
        ///     This method has no functionality unless the method is overridden
        /// </summary>
        public virtual void Run()
        {
        }

        /// <summary>
        ///     Causes the operating system to change the state of the current thread instance to ThreadState.Running
        /// </summary>
        public void Start()
        {
            thread.Start();
        }

        /// <summary>
        ///     Interrupts a thread that is in the WaitSleepJoin thread state
        /// </summary>
        protected void Interrupt()
        {
            thread.Interrupt();
        }

        /// <summary>
        ///     Blocks the calling thread until a thread terminates
        /// </summary>
        public void Join()
        {
            thread.Join();
        }

        /// <summary>
        ///     Obtain a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Thread[{0},{1},]", Name, Priority);
        }
    }
}