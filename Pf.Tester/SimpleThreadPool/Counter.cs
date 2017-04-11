using System.Threading;

namespace Pf.Tester.SimpleThreadPool
{
    /// <summary>
    /// 计数器。任务处理状态标志，当处理任务数过大时暂停处理，优先处理根目录下文件。
    /// </summary>
    public class Counter
    {
        /// <summary>
        /// The _max number
        /// </summary>
        private readonly long _maxNumber;
        /// <summary>
        /// The _task number
        /// </summary>
        private long _taskNumber = 0;

        /// <summary>
        /// 计数器构造
        /// </summary>
        /// <param name="maxNumber">计数的最大值.</param>
        public Counter(long maxNumber)
        {
            this._maxNumber = maxNumber;
        }

        /// <summary>
        /// 计数器递增加一
        /// </summary>
        public void Increment()
        {
            // taskNumber++;
            Interlocked.Increment(ref _taskNumber);
        }
        /// <summary>
        /// 计数器递增减一
        /// </summary>
        public void Decrement()
        {
            // taskNumber++;
            Interlocked.Decrement(ref _taskNumber);
        }
        /// <summary>
        /// 计数器递增加一并且返回计数器是否达到最大值
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool IncrementAndIsMaxed()
        {
            Increment();
            return IsMaxed();
        }

        /// <summary>
        /// 返回计数器是否达到最大值
        /// </summary>
        /// <returns><c>true</c> if this instance is maxed; otherwise, <c>false</c>.</returns>
        public bool IsMaxed()
        {
            return _taskNumber >= _maxNumber;
        }

        /// <summary>
        /// 重置计数器
        /// </summary>
        public void Reset()
        {
            _taskNumber = -1;
        }
    }
}