using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AntWall
{
    class BasicInfo
    {
        static PerformanceCounter CpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        static PerformanceCounter RAMounter = new PerformanceCounter("Memory", "Available Bytes");

        static Dictionary<string, PerformanceCounter> CPUCounters = new Dictionary<string, PerformanceCounter>();

        public static void Init()
        {
            CpuCounter.NextValue();

            RAMounter.NextValue();
        }

        public static double GetCPUProcessUsage(string name)
        {
            if (!CPUCounters.ContainsKey(name))
            {
                CPUCounters[name] = new PerformanceCounter("Process", "% Processor Time", name);
                CPUCounters[name].NextValue();
            }

            return CPUCounters[name].NextValue() / Environment.ProcessorCount;
        }


        public string Username = Environment.UserName;
        public string Hostname = Environment.MachineName;

        public string OS = Environment.OSVersion.ToString();
        public bool Is64BitOS = Environment.Is64BitOperatingSystem;
        
        public ulong RAMSize = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
        public ulong FreeRam
        {
            get
            {
                return (ulong)RAMounter.NextValue();
            }
        }

        public TimeSpan UpTime = TimeSpan.FromMilliseconds(Environment.TickCount);
		public double UpTimeSeconds 
		{
			get
			{
				return UpTime.TotalSeconds;
			}
		}
			
		public DateTime StartTime 
		{
			get
			{
				return DateTime.Now - UpTime;
			}
		}

        //public TimeSpan UpTime
        //{
        //    get
        //    {
        //        PerformanceCounter systemUpTime = new PerformanceCounter("System", "System Up Time");
        //        systemUpTime.NextValue();
        //        return TimeSpan.FromSeconds(systemUpTime.NextValue());
        //    }
        //}

        public int ProcessorCount = Environment.ProcessorCount;
        public double TotalCPUUsage
        {
            get
            {
                return CpuCounter.NextValue();
            }
        }

        public static string Get(dynamic args)
        {
            return JsonConvert.SerializeObject(new BasicInfo());
        }
    }
}
