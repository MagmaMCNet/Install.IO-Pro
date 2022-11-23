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
            Application.Run(new InstallForm());
        }
    }
}