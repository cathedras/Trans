using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace myzy.Util
{
    public class HiPerfTimer
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        private long startTime;
        private long stopTime;
        private long freq;

        /// <summary>
        /// Constructor
        /// </summary>
        public HiPerfTimer()
        {
            startTime = 0;
            stopTime = 0;

            if (QueryPerformanceFrequency(out freq) == false)
            {
                // high-performance counter not supported 
                throw new Win32Exception();
            }
        }

        /// <summary>
        /// reset the timer
        /// </summary>
        public void Reset()
        {
            startTime = 0;
            stopTime = 0;
        }
        /// <summary>
        /// Start the timer
        /// </summary>
        public void Start()
        {
            // lets do the waiting threads there work
            // Thread.Sleep(0);

            QueryPerformanceCounter(out startTime);
        }

        /// <summary>
        /// Stop the timer
        /// </summary>
        private void Stop()
        {
            QueryPerformanceCounter(out stopTime);
        }

        /// <summary>
        /// Returns the duration of the timer (in seconds)
        /// </summary>
        public double Duration
        {
            get
            {
                QueryPerformanceCounter(out stopTime);
                return (double)(stopTime - startTime) / (double)freq;
            }
        }

        /// <summary>
        /// Returns the duration of the timer (in seconds)
        /// </summary>
        public int DurationMillisecond
        {
            get
            {
                return (int)Duration * 1000;
            }
        }

        public static void Delay(uint ms)
        {
            HiPerfTimer hi = new HiPerfTimer();

            var delay = (double)ms / 1000;
            hi.Start();

            while (true)
            {
                if (hi.Duration > delay)
                {
                    break;
                }
            }
        }
    }
}
