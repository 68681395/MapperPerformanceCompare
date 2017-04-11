using PerformanceTool.Osgi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
[assembly: PerformanceTool.Osgi.RegExtensionPoint(typeof(LazyLoading))]
namespace PerformanceTool.Osgi
{
    /// <summary>
    ///     晚加载创建工厂
    /// </summary>
    /// <author>
    ///     tangjingbo
    /// </author>
    public class LazyLoading : ExtensionPoint<RegLazyLoadingAttribute>
    {
        private static readonly SimpleLogger.ILog log = SimpleLogger.LogManager.GetLogger("LazyLoading");
        private static readonly Hashtable LazyLoadings = Hashtable.Synchronized(new Hashtable(100));

        /// <summary>
        ///     注册晚加载关系
        /// </summary>
        /// <param name="intfType">接口类型或抽象类</param>
        /// <param name="implType">实现intfType的最终类型，必须有一个默认的无参构造类</param>
        /// <param name="priority">优先级，总是使用优先级最高的实现类</param>
        public static void RegisterLazyLoading(
            Type intfType, Type implType, LoadingPriority priority)
        {
            if (intfType == null)
                throw new ArgumentNullException("intfType", "RegLazyLoadingAttribute 参数1不能为null。");
            if (implType == null)
                throw new ArgumentNullException("implType", "RegLazyLoadingAttribute 参数2不能为null。");
            if (!intfType.IsAssignableFrom(implType))
                throw new ArgumentException(string.Format("类‘{0}’必须实现接口‘{1}'", implType, intfType));
            if (implType.GetConstructor(new Type[0]) == null)
                throw new ArgumentException(string.Format("类‘{0}’没有默认构造函数", implType));

            var list = (ImplCollection)LazyLoadings[intfType];
            if (list == null)
            {
                list = new ImplCollection();
                LazyLoadings.Add(intfType, list);
            }
            list.Add(implType, priority);
        }

        /// <summary>
        ///     注销后期绑定关系，如果该类型被多次注册，将移除优先级最低的
        /// </summary>
        /// <param name="intfType">接口类型或抽象类</param>
        /// <param name="implType">实现intfType的最终类型</param>
        public static void UnRegisterLazyLoading(Type intfType, Type implType)
        {
            var list = (ImplCollection)LazyLoadings[intfType];
            if (list != null)
                list.Remove(implType);
        }

        /// <summary>
        ///     获取已注册晚加载类型，通过反射机制创建该类型实例
        ///     如果类型T没有注册，将抛出LazyLoadingException
        /// </summary>
        /// <typeparam name="T">接口类型或抽象类</typeparam>
        /// <returns>实现intfType的对象</returns>
        public static T New<T>()
        {
            var list = (ImplCollection)LazyLoadings[typeof(T)];
            if (list == null || list.Count == 0)
            {
                var ex = new Exception(typeof(T).FullName + "没有注册实现类");
                log.Error("没有注册实现类", ex);
                throw ex;
            }
            return (T)list.GetHighestImpl();
        }

        /// <summary>
        ///     指示是否注册了类型 T 的实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns><c>true</c> if this instance has resitered; otherwise, <c>false</c>.</returns>
        public static bool HasImpls<T>()
        {
            var list = (ImplCollection)LazyLoadings[typeof(T)];
            return list != null && list.Count > 0;
        }

        /// <summary>
        ///     获取已注册绑定关系的实现类型，通过反射机制创建该类型实例
        ///     如果类型T没有注册，将抛出ArgumentOutOfRangeException
        /// </summary>
        /// <typeparam name="T">接口类型或抽象类</typeparam>
        /// <returns>实现intfType的对象</returns>
        public static List<T> NewAll<T>()
        {
            var list = (ImplCollection)LazyLoadings[typeof(T)];
            if (list == null || list.Count == 0)
                return new List<T>();
            return list.GetAllImpls<T>();
        }

        public static List<KeyValuePair<TMetadata, T>> NewAll<TMetadata, T>()
        {
            var list = (ImplCollection)LazyLoadings[typeof(T)];
            if (list == null || list.Count == 0)
                return new List<KeyValuePair<TMetadata, T>>();
            return list.GetAllImpls<TMetadata, T>();
        }

        #region Nested type: ImplCollection

        private class ImplCollection
        {
            private readonly List<Implement> _list = new List<Implement>(1);
            private bool _sorted = true;

            public int Count
            {
                get { return _list.Count; }
            }

            public void Add(Type implType, LoadingPriority priority)
            {
                _list.Add(new Implement(implType, priority));
                _sorted = _list.Count < 2;
            }

            public void Remove(Type implType)
            {
                if (!_sorted) _list.Sort();
                for (int i = 0, c = _list.Count; i < c; i++)
                {
                    var item = _list[i];
                    if (item.ImplType == implType)
                    {
                        _list.RemoveAt(i);
                        break;
                    }
                }
            }

            public object GetHighestImpl()
            {
                if (!_sorted) _list.Sort();
                return _list.Count > 0 ? _list[_list.Count - 1].New() : null;
            }

            /// <summary>
            ///     获取所有实现，并且以优先级自高到低排序
            /// </summary>
            /// <returns></returns>
            internal List<T> GetAllImpls<T>()
            {
                if (!_sorted) _list.Sort();
                var result = new List<T>();
                for (var i = _list.Count - 1; i >= 0; i--)
                    result.Add((T)_list[i].New());
                return result;
            }

            internal List<KeyValuePair<TMetadata, T>> GetAllImpls<TMetadata, T>()
            {
                if (!_sorted) _list.Sort();
                var result = new List<KeyValuePair<TMetadata, T>>();
                for (var i = _list.Count - 1; i >= 0; i--)
                {
                    var item = _list[i];
                    result.Add(new KeyValuePair<TMetadata, T>(item.GetMetadata<TMetadata>(), (T)item.New()));
                }
                return result;
            }
        }

        #endregion

        #region Nested type: Implement

        private class Implement : IComparable<Implement>
        {
            private static Type atrributetype = typeof(Attribute);

            private Func<object> _new;

            public Implement(Type implType, LoadingPriority priority)
            {
                ImplType = implType;
                Priority = (byte)priority;
                metadatas = ImplType.GetCustomAttributes<Attribute>(false).ToArray();
            }

            public Func<object> New
            {
                get
                {
                    return _new ??
                           (_new =
                               Expression.Lambda<Func<object>>(Expression.New(ImplType)).Compile());
                }
            }

            public Type ImplType { get; }
            public byte Priority { get; }

            #region IComparable<Implement> Members

            public int CompareTo(Implement other)
            {
                var result = Priority - other.Priority;
                if (result == 0)
                    result = string.CompareOrdinal(ImplType.FullName, other.ImplType.FullName);
                return result;
            }

            private readonly Attribute[] metadatas;

            internal TMetadata GetMetadata<TMetadata>()
            {
                return metadatas.Cast<TMetadata>().FirstOrDefault();
            }

            #endregion
        }

        #endregion

        #region IExtensionPoint 成员

        /// <summary>
        ///     Registers the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="attr">The attr.</param>
        protected override void Register(Assembly assembly, RegLazyLoadingAttribute attr)
        {
            RegisterLazyLoading(attr.IntfType, attr.ImplType, attr.Priority);
        }

        /// <summary>
        ///     Uns the register.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="attr">The attr.</param>
        protected override void UnRegister(Assembly assembly, RegLazyLoadingAttribute attr)
        {
            UnRegisterLazyLoading(attr.IntfType, attr.ImplType);
        }

        #endregion
    }
}