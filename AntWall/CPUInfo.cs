using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json;
using System.Collections;

namespace AntWall
{
    class CPUInfo
    {
        const int Resolution = 250;
        const int NSamples = 4;

        public static double Calculate(CounterSample oldSample, CounterSample newSample)
        {
            double difference = newSample.RawValue - oldSample.RawValue;
            double timeInterval = newSample.TimeStamp100nSec - oldSample.TimeStamp100nSec;
            if (timeInterval != 0) return difference / timeInterval;
            return 0;
        }

        static SinglePCInfo Info;
        static Timer Timer;

        public static void Init()
        {
            Info = new SinglePCInfo();
            Timer = new Timer(_ => Tick(), null, 100, Resolution);
        }

        static void Tick()
        {
            lock (Info)
            {
                Info.Tick();
            }
        }

        public static string Get(dynamic args)
        {
            lock (Info)
            {
                return JsonConvert.SerializeObject(Info);
            }
        }

        class ShiftingCollection<T> : IEnumerable<T>
        {
            T[] Items;
            int Next = 0;

            public ShiftingCollection(int NItems)
            {
                Items = new T[NItems];
            }

            public void Add(T item)
            {
                Items[Next] = item;
                Next++;
                if (Next >= Items.Length) Next = 0;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return Collection.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return Collection.GetEnumerator();
            }

            IEnumerable<T> Collection
            {
                get
                {
                    int i = Next;
                    do
                    {
                        i--;
                        if (i < 0) i = Items.Length - 1;
                        yield return Items[i];
                    } while (i != Next);
                }
            }

        }

        class SingleCoreInfo
        {
            public int Core_ID;
            public ShiftingCollection<double> Results = new ShiftingCollection<double>(NSamples);

            PerformanceCounter PC;

            public SingleCoreInfo(int cpu_id, int core_id)
            {
                Core_ID = core_id;
                PC = new PerformanceCounter("Processor Information", "% Processor Time", cpu_id + "," + core_id, true);
            }

            public void Tick()
            {
                Results.Add(PC.NextValue());
            }
        }

        class SingleCPUInfo
        {
            public int CPU_ID;
            public ShiftingCollection<double> Total = new ShiftingCollection<double>(NSamples);
            public SingleCoreInfo[] Cores;

            PerformanceCounter PC;

            public SingleCPUInfo(int cpu_id, int ncores)
            {
                CPU_ID = cpu_id;
                PC = new PerformanceCounter("Processor Information", "% Processor Time", cpu_id + ",_Total", true);
                Cores = Enumerable.Range(0, ncores).Select(i => new SingleCoreInfo(cpu_id, i)).ToArray();
            }

            public void Tick()
            {
                Total.Add(PC.NextValue());

                foreach (var item in Cores)
                {
                    item.Tick();
                }
            }
        }

        class SinglePCInfo
        {
            public int ResolutionMS = Resolution;
            public DateTime LastTick;
            public ShiftingCollection<double> Total = new ShiftingCollection<double>(NSamples);
            public SingleCPUInfo[] CPUs;

            PerformanceCounter PC;

            public SinglePCInfo()
            {
                PC = new PerformanceCounter("Processor Information", "% Processor Time", "_Total", true);
                var cpuCores = new Dictionary<int, int>();
                foreach (var item in new PerformanceCounterCategory("Processor Information").GetInstanceNames())
                {
                    var splat = item.Split(',');
                    int cpuId, coreId;
                    if(splat.Length == 2 && int.TryParse(splat[0], out cpuId) && int.TryParse(splat[1], out coreId))
                    {
                        if (!cpuCores.ContainsKey(cpuId)) cpuCores[cpuId] = 1;
                        else cpuCores[cpuId]++;
                    }
                }

                CPUs = cpuCores.Select(c => new SingleCPUInfo(c.Key, c.Value)).ToArray();
            }

            public void Tick()
            {
                Total.Add(PC.NextValue());
                foreach (var item in CPUs)
                {
                    item.Tick();
                }
                LastTick = DateTime.Now;
            }
        }
    }
}
