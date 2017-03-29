using System;
using System.Diagnostics;
using System.Reflection;
using Tsharp;
using TSharp.Core.Osgi;

namespace Common.Logging
{
    internal interface ILog
    {
        void Error(string v, Exception ex);
        void Warn(string v);
        void Error(Exception ex);
        void Warn(string v, Exception ex);
        void ErrorFormat(string v, Exception ex, params object[] args);
    }

    class Log : ILog
    {
        public void Error(Exception ex)
        {
            SimpleLogger.Default.WriteLine(ex.ToString());
        }

        public void Error(string v, Exception ex)
        {
            SimpleLogger.Default.WriteLine($"[Error] {v} {ex}");
        }

        public void ErrorFormat(string format, Exception ex, params object[] args)
        {
            SimpleLogger.Default.WriteLine($"[Error] {string.Format(format, args)} {ex.ToString()}");
        }

        public void Warn(string v)
        {
            SimpleLogger.Default.WriteLine($"[Warn] {v}");
        }

        public void Warn(string v, Exception ex)
        {
            SimpleLogger.Default.WriteLine($"[Warn] {v} {ex}");
        }
    }

    internal class LogManager
    {
        internal static ILog GetCurrentClassLogger()
        {
            StackFrame frame = new StackFrame(1, false);
            return GetLogger(frame.GetMethod().ReflectedType.FullName);
        }

        internal static ILog GetLogger(string v)
        {
            return new Log();
        }
    }
}