using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace AntWall
{
    class Processes
    {
        class ProcessInfo
        {
            public string Name;
            public int Id;
            public long PrivateMemorySize;
            public long WorkingSet;
            public DateTime StartTime;
            public TimeSpan TotalProcessorTime;
            public TimeSpan PrivilegedProcessorTime;
            public TimeSpan UserProcessorTime;
            public int Threads;
            public string PriorityClass;
            public int BasePriority;
            public bool IsResponding;
            public string MainWindowTitle;
            public double CPUUsage;
        }

        public static Dictionary<int, TimeSpan> LastProcessorTimes = new Dictionary<int, TimeSpan>();
        public static DateTime LastTime = DateTime.Now;

        public static string Get(dynamic args)
        {
            lock (LastProcessorTimes)
            {
                var delta = DateTime.Now - LastTime;
                LastTime = DateTime.Now;

                var processes = Process.GetProcesses().Select(p =>
                {
                    try
                    {
                        var pr = new ProcessInfo
                        {
                            Name = p.ProcessName,
                            Id = p.Id,
                            PrivateMemorySize = p.PrivateMemorySize64,
                            WorkingSet = p.WorkingSet64,
                            StartTime = p.StartTime,
                            TotalProcessorTime = p.TotalProcessorTime,
                            PrivilegedProcessorTime = p.PrivilegedProcessorTime,
                            UserProcessorTime = p.UserProcessorTime,
                            Threads = p.Threads.Count,
                            PriorityClass = p.PriorityClass.ToString(),
                            BasePriority = p.BasePriority,
                            IsResponding = p.Responding,
                            MainWindowTitle = p.MainWindowTitle,
                        };

                        if (LastProcessorTimes.ContainsKey(p.Id)) pr.CPUUsage = (p.TotalProcessorTime - LastProcessorTimes[p.Id]).TotalMilliseconds / delta.TotalMilliseconds / Environment.ProcessorCount;

                        LastProcessorTimes[p.Id] = p.TotalProcessorTime;

                        return pr;
                    }
                    catch (Win32Exception) { }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    return null;
                }).Where(p => p != null).ToList();
                return JsonConvert.SerializeObject(processes);
            }
        }
    }
}
