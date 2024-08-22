using GuerrillaNtp;
using System;
using System.Threading.Tasks;

namespace Clock
{
    internal class UTCTimeProvider
    {
        private static NtpClock clock;
        private static DateTime _nextSecond;
        public static event Action<DateTime> OnFullSecond;
        private static System.Timers.Timer timer;

        static UTCTimeProvider()
        {
            NtpClient client = NtpClient.Default;
            Sync(client);
            Microsoft.Win32.SystemEvents.TimeChanged += (object o, EventArgs e) => Sync(client);
        }

        private static void Sync(NtpClient client)
        {
            var x = client.QueryAsync();
            x.ContinueWith(OnQueryComplete);
        }

        private static void OnQueryComplete(Task<NtpClock> task)
        {
            clock = task.Result;
            var serverTime = clock.UtcNow.DateTime;
            _nextSecond = serverTime.AddSeconds(1).AddMilliseconds(-serverTime.Millisecond);

            // Calculate the initial delay to align with the next full second
            int initialDelay = (int)(_nextSecond - serverTime).TotalMilliseconds;

            // Start the timer to fire at every full second after the initial delay
            timer = new System.Timers.Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = false;
            timer.Start();
        }

        private static void ScheduleNextTick()
        {
            // Calculate the time until the next full second
            var now = clock.UtcNow.DateTime;
            DateTime nextSecond = now.AddSeconds(1).AddTicks(-now.Ticks % TimeSpan.TicksPerSecond);

            TimeSpan timeToNextSecond = nextSecond - now;
            timer.Interval = timeToNextSecond.TotalMilliseconds;

            timer.Start(); // Start the timer
        }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var serverTime = clock.UtcNow.DateTime;
            OnFullSecond?.Invoke(serverTime);
            ScheduleNextTick();
        }
    }
}
