using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Watchdog
{
    class Program
    {
        private static string fullPathExecutable;
        static void Main(string[] args)
        {
            bool show_help = false;
            bool respawn = false;
            int maxCPUAverageUsage = 90;
            string processName = "";

            var p = new OptionSet() {
                { "n|name=", "the name of the Process you want to watch.", v => processName = v },
                { "r|respawn", "if Process is died, should we respawn the mother process (be carefull if you watch a child process)?", v => respawn= v != null },
                { "c|cpu=", "the number of maximal CPU usage in average for next 10 Minutes. this must be an integer.", (int v) => maxCPUAverageUsage = v },
                { "h|help",  "show this message and exit", v => show_help = v != null },
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("Watchdog: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `greet --help' for more information.");
            }

            if (show_help || processName.Equals(""))
            {
                ShowHelp(p);
                return;
            }
            RunWatcher(processName, maxCPUAverageUsage, respawn);
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: watchdog [OPTIONS]");
            Console.WriteLine("Watch declared tasks and end it, if a average CPU load was hit in the last 10 minutes.");
            Console.WriteLine("If no cpu option is giving the watchdogs is set to 90.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
    
        static void RunWatcher(String processName, int maxCPUAverageUsage, bool respawn)
        {
            while (!Console.KeyAvailable)
            {
                Process[] pp = Process.GetProcessesByName(processName);
                if (pp.Length == 0 && !respawn && Program.fullPathExecutable == null)
                {
                    Console.WriteLine(processName + " does not exist");
                    System.Environment.Exit(1);
                }

                if(respawn && Program.fullPathExecutable != null)
                {
                    Respawn();
                }

                Program.fullPathExecutable = pp[0].MainModule.FileName;
                foreach (Process proc in pp)
                {
                    Watcher watchdog = new Watcher(proc, maxCPUAverageUsage);
                    new Thread(new ThreadStart(watchdog.Watch)).Start();
                }
                Thread.Sleep(1000 * 60 * 20);
            }
        }

        static void Respawn()
        {
            Process.Start(Program.fullPathExecutable);
        }
    }
}