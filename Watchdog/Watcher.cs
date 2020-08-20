using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Watchdog
{
    class Watcher
    {
        private static double average;
        private static DateTime lastTime;
        private static TimeSpan lastTotalProcessorTime;
        private static DateTime curTime;
        private static TimeSpan curTotalProcessorTime;
        private Process p;
        private int maxCPUAverageUsage;
        private int timeLimit;

        public Watcher(Process p, int maxCPUAverageUsage, int timeLimit)
        {
            this.p = p;
            this.maxCPUAverageUsage = maxCPUAverageUsage;
            this.timeLimit = timeLimit;
        }

        public void Watch()
        {
            var processorCollection = new List<double>();

            if (lastTime == null || lastTime == new DateTime())
            {
                lastTime = DateTime.Now;
                lastTotalProcessorTime = p.TotalProcessorTime;
            }
            else
            {
                for (int livetime = 1; livetime < timeLimit * 60; livetime++)
                {
                    curTime = DateTime.Now;
                    curTotalProcessorTime = p.TotalProcessorTime;

                    double CPUUsage = (curTotalProcessorTime.TotalMilliseconds - lastTotalProcessorTime.TotalMilliseconds) / curTime.Subtract(lastTime).TotalMilliseconds / Convert.ToDouble(Environment.ProcessorCount);
                    processorCollection.Add(CPUUsage * 100);
                    average = processorCollection.Average();
                    if (average >= maxCPUAverageUsage && processorCollection.Count >= 1000)
                    {
                        p.Kill();
                        Console.WriteLine("Kill Processes - cause of high load");
                    }
                    if (processorCollection.Count >= 1000)
                    {
                        processorCollection.Clear();
                    }
                    lastTime = curTime;
                    lastTotalProcessorTime = curTotalProcessorTime;

                    Thread.Sleep(1000 * 60);
                }
            }
        }
    }
}
