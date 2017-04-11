using System;

namespace PerformanceTool.Osgi
{
    /// <summary>
    ///     扩展标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RegExtensionAttribute : Attribute, IComparable<RegExtensionAttribute>
    {
        /// <summary>
        ///     序号，注册时指定的一个整数
        /// </summary>
        /// <value>
        ///     The order.
        /// </value>
        /// <remarks>
        ///     TODO:目前Order属性仅作为传递值，还没有做到将按order属性执行Register方法
        /// </remarks>
        public int Order { get; set; } = 0;

        /// <summary>
        ///     Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///     A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the
        ///     following meanings: Value Meaning Less than zero This object is less than the other parameter.Zero This object is
        ///     equal to other. Greater than zero This object is greater than other.
        /// </returns>
        public int CompareTo(RegExtensionAttribute other)
        {
            if (other != null)
                return Order.CompareTo(other.Order);
            return 1;
        }
    }
}