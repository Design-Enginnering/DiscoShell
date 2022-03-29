using System.Text;

namespace discosh.Protections
{
    public class UTF16BOM
    {
        public static byte[] Process(string input)
        {
            byte[] uniheader = new byte[] { 0xFF, 0xFE, 0x0D, 0x0A };
            byte[] gencodebytes = Encoding.ASCII.GetBytes(input);
            byte[] ret = new byte[uniheader.Length + gencodebytes.Length + 1];
            uniheader.CopyTo(ret, 0);
            gencodebytes.CopyTo(ret, uniheader.Length);
            return ret;
        }
    }
}
