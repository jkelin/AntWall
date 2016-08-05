using CommandLine;
using CommandLine.Text;
using Nancy;
using Nancy.Hosting.Self;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Wallpainter;

namespace AntWall
{
    public class ApiModule : NancyModule
    {
        public static Func<dynamic, Response> JSON(Func<dynamic, string> action)
        {
            return args =>
            {
                var jsonBytes = Encoding.UTF8.GetBytes(action(args));
                return new Response
                {
                    ContentType = "application/json",
                    Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
                };
            };
        }

        public ApiModule()
        {
            Get["/api/fullscreen/{monitor}/{title}"] = args =>
            {
                Console.WriteLine("Setting fullscreen background on mon {0} for window {1}", args.monitor, args.title);
                var mgr = new WallpaperManager();
                Program.Managers.Add(mgr);
                IntPtr wndHandle = WinAPI.FindWindow(null, args.title);
                mgr.SetWallpaper(wndHandle, args.monitor);
                return "done";
            };

            Get["/glue.js"] = _ => new Response
            {
                ContentType = "application/javascript",
                Contents = s => s.Write(Program.Glue, 0, Program.Glue.Length)
            };

            Get["/api/basic_info.json"] = JSON(BasicInfo.Get);
            Get["/api/processes.json"] = JSON(Processes.Get);
            Get["/api/cpu_info.json"] = JSON(CPUInfo.Get);
        }
    }

    class Options
    {
        [Option('p', "port", DefaultValue = (ushort)39583, HelpText = "Port to listen on")]
        public ushort Port { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }

    class Program
    {
        public static byte[] Glue;
        public static List<WallpaperManager> Managers = new List<WallpaperManager>();

        static byte[] ReadResource(string resource)
        {
            using (var ms = new MemoryStream())
            {
                using (var res = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                {
                    res.CopyTo(ms);
                }

                return ms.ToArray();
            }
        }

        static void Main(string[] args)
        {
            Glue = ReadResource("AntWall.glue.js");
            var options = new Options();
            if (Parser.Default.ParseArguments(args, options))
            {
                BasicInfo.Init();
                CPUInfo.Init();
                Processes.Get("");
                var config = new HostConfiguration() { UrlReservations = new UrlReservations() { CreateAutomatically = true } };
                using (var host = new NancyHost(config, new Uri("http://localhost:" + options.Port + "/")))
                {
                    host.Start();
                    Console.WriteLine("Listening on port {0}", options.Port);
                    Console.WriteLine("Press enter to stop");
                    Console.ReadLine();
                }

                Managers.ForEach(m => m.Dispose());
            }
        }
    }
}
