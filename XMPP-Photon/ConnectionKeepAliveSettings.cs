using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace com.negociosit.XMPP.XMPPphoton
{
    static class Repeat
    {
        public static Task Interval(TimeSpan pollInterval, Action action, System.Threading.CancellationToken token)
        {
            return Task.Factory.StartNew(() =>
            {
                for (; ; )
                {
                    if (token.WaitCancellationRequested(pollInterval))
                        break;
                    action();
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
    }
    static class CancellationTokenExtensions
    {
        public static bool WaitCancellationRequested(
            this CancellationToken token,
            TimeSpan timeout)
        {
            return token.WaitHandle.WaitOne(timeout);
        }
    }
}
