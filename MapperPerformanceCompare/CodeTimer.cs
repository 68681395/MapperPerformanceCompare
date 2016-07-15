using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace NLiteEmitCompare
{
    public sealed class CodeTimer
    {
        public static void Initialize()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            Time(null, () => { }, null, 1);
        }

        public static void Time(ITestMetadata mapper, Action action)
        {
            Time(null, action, null, 1);
        }

        public static void Time(ITestMetadata mapper, Action action, List<Record> records, params int[] iterations)
        {
            if (mapper == null) return;

            // 1.
            ConsoleColor currentForeColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(mapper.Name);
            var record = new Record()
            {
                Mapper = mapper,
                Iterations = new List<IterationRecord>()
            };
            foreach (var iteration in iterations)
            {
                // 2.
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                int[] gcCounts = new int[GC.MaxGeneration + 1];
                for (int i = 0; i <= GC.MaxGeneration; i++)
                {
                    gcCounts[i] = GC.CollectionCount(i);
                }
                // 3.
                Stopwatch watch = new Stopwatch();
                watch.Reset();
                watch.Start();
                long cycleCount = GetCycleCount();
                for (int i = 0; i < iteration; i++) action();
                long cpuCycles = GetCycleCount() - cycleCount;
                watch.Stop();
                if (mapper != null)
                    record.Iterations.Add(new IterationRecord()
                    {
                        Iteration = iteration,
                        CPUCycles = cpuCycles,
                        TimeElapsed = watch.ElapsedMilliseconds
                    });
                // 4.
                // Console.ForegroundColor = currentForeColor;
                // Console.WriteLine("\tTime Elapsed:\t" + watch.ElapsedMilliseconds.ToString("N0") + "ms");
                //  Console.WriteLine("\tCPU Cycles:\t" + cpuCycles.ToString("N0"));
                Console.Title = mapper.Name + "   " + iteration + "/" + iterations[iterations.Length - 1];

                // 5.
                for (int i = 0; i <= GC.MaxGeneration; i++)
                {
                    int count = GC.CollectionCount(i) - gcCounts[i];
                    // Console.WriteLine("\tGen " + i + ": \t\t" + count);
                }
            }
            records.Add(record);
            Console.WriteLine();

        }

        private static long GetCycleCount()
        {
            //ulong cycleCount = 0;
            //QueryThreadCycleTime(GetCurrentThread(), ref cycleCount);
            //return cycleCount;
            return GetCurrentThreadTimes();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetThreadTimes(IntPtr hThread, out long lpCreationTime,
            out long lpExitTime, out long lpKernelTime, out long lpUserTime);

        private static long GetCurrentThreadTimes()
        {
            long l;
            long kernelTime, userTimer;
            GetThreadTimes(GetCurrentThread(), out l, out l, out kernelTime,
                out userTimer);
            return kernelTime + userTimer;
        }


        //[DllImport("kernel32.dll")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //static extern bool QueryThreadCycleTime(IntPtr threadHandle, ref ulong cycleTime);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentThread();

    }
}