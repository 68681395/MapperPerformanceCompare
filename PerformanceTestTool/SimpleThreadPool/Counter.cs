using System.Threading;

namespace Pf.Tester.SimpleThreadPool
{
    /// <summary>
    /// ��������������״̬��־������������������ʱ��ͣ�������ȴ����Ŀ¼���ļ���
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
        /// ����������
        /// </summary>
        /// <param name="maxNumber">���������ֵ.</param>
        public Counter(long maxNumber)
        {
            this._maxNumber = maxNumber;
        }

        /// <summary>
        /// ������������һ
        /// </summary>
        public void Increment()
        {
            // taskNumber++;
            Interlocked.Increment(ref _taskNumber);
        }
        /// <summary>
        /// ������������һ
        /// </summary>
        public void Decrement()
        {
            // taskNumber++;
            Interlocked.Decrement(ref _taskNumber);
        }
        /// <summary>
        /// ������������һ���ҷ��ؼ������Ƿ�ﵽ���ֵ
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool IncrementAndIsMaxed()
        {
            Increment();
            return IsMaxed();
        }

        /// <summary>
        /// ���ؼ������Ƿ�ﵽ���ֵ
        /// </summary>
        /// <returns><c>true</c> if this instance is maxed; otherwise, <c>false</c>.</returns>
        public bool IsMaxed()
        {
            return _taskNumber >= _maxNumber;
        }

        /// <summary>
        /// ���ü�����
        /// </summary>
        public void Reset()
        {
            _taskNumber = -1;
        }
    }
}