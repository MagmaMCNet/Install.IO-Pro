using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using MagmaMc.JEF;
namespace Install.IO_Pro
{
    internal static class Startup
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AttachConsole(int a);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FreeConsole();

        [MTAThread]
        static void Main()
        {
            if (!File.Exists("SplashScreen.exe"))
                using (WebClient Client = new())
                {
                    Client.DownloadFile("http://www.magma-mc.net:5555/Files/Binaries/SplashScreen.7z.exe", "PS7Z.exe");
                    ProcessStartInfo startInfo = new ProcessStartInfo("PS7Z.exe", "-y") { WindowStyle = ProcessWindowStyle.Minimized };
                    Process? p = Process.Start(startInfo);
                    if (p != null)
                        p.WaitForExit();
                    File.Delete("PS7Z.exe");
                }
            
            string[] ConsoleArgs = Environment.GetCommandLineArgs();
            bool encrypt = JEF.Lists.IsInList(ConsoleArgs.ToList(), "-e", false);
            bool decrypt = JEF.Lists.IsInList(ConsoleArgs.ToList(), "-d", false);
            if (!encrypt)
                encrypt = JEF.Lists.IsInList(ConsoleArgs.ToList(), "--encrypt", false);
            if (!decrypt)
                decrypt = JEF.Lists.IsInList(ConsoleArgs.ToList(), "--decrypt", false);

            if (encrypt)
            {
                if (!AttachConsole(-1)) AllocConsole();
                Console.WriteLine("asd");
                CommonUtils.Encrpter.Init();
                CommonUtils.Encrpter.Secure("Data/", "data", null);

                Console.WriteLine("Press Enter To Exit");
                FreeConsole();
                Environment.Exit(0);
            }

            Application.EnableVisualStyles();
            ApplicationConfiguration.Initialize();
            Application.Run(new InstallForm());
        }
    }
}