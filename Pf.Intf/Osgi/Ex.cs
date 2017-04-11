using System;
using System.Collections.Generic;

namespace PerformanceTool.Osgi
{
    internal static class Ex
    {
        public static void Foreach<T>(this IEnumerable<T> es, Action<T> ac)
        {
            foreach (var oe in es)
                ac(oe);
        }
    }
}