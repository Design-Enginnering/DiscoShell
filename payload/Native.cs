using System.Runtime.InteropServices;

namespace payload
{
    public class Native
    {
        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);
    }
}
