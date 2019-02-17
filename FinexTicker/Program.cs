using System.Threading;
using System.Threading.Tasks;

namespace FinexTicker
{
    class Program
    {
        static void Main(string[] args)
        {
            var exitEvent = new ManualResetEvent(false);
            using (var app = new App())
            {
                app.Start();
                exitEvent.WaitOne();
            }
        }
    }
}
