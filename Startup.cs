using MagmaMc.JEF;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Install.IO_Pro
{
    internal static class Startup
    {
        [MTAThread]
        static void Main()
        {
            string[] ConsoleArgs = Environment.GetCommandLineArgs();
            Application.EnableVisualStyles();
            ApplicationConfiguration.Initialize();
<<<<<<< HEAD
            Application.Run(new InstallForm());
=======
            if (JEF.Lists.IsInList(ConsoleArgs.ToList(), "--setup") || JEF.Lists.IsInList(ConsoleArgs.ToList(), "-s"))
                Application.Run(new PublisherSetup());
            else
                Application.Run(new InstallForm());
>>>>>>> 0e28a583dbe8804c8a7b9051200bc83ad338929b
        }
    }
}