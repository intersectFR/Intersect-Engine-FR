using System;

using Intersect.Extensions;
using Intersect.GameObjects;
using Intersect.Server.Networking;
using Intersect.Utilities;

namespace Intersect.Server.General
{

    public static class Time
    {

        public static DateTime GameTime { get; set; }

        private static int sTimeRange;

        private static long sUpdateTime;

        public static string Hour = "00";
        public static string MilitaryHour = "00";
        public static string Minute = "00";
        public static string Second = "00";


        public static void Init()
        {
            var timeBase = TimeBase.GetTimeBase();
            var now = Timing.Global.Now;
            if (timeBase.SyncTime)
            {
                GameTime = now;
            }
            else
            {
                GameTime = new DateTime(
                    now.Year,
                    now.Month,
                    now.Day,
                    Randomization.Next(0, 24),
                    Randomization.Next(0, 60),
                    Randomization.Next(0, 60)
                );
            }

            sTimeRange = -1;
            sUpdateTime = 0;
        }

        public static void Update()
        {
            var timeBase = TimeBase.GetTimeBase();
            var lastTicks = GameTime.Ticks;
            if (timeBase.SyncTime)
            {
                GameTime = Timing.Global.Now;
            }
            else
            {
                while (Timing.Global.Milliseconds > sUpdateTime)
                {
                    GameTime = GameTime.AddMilliseconds(1000 * timeBase.Rate);
                    sUpdateTime += 1000;
                }
            }

            if (GameTime.Ticks == lastTicks)
            {
                // Short circuit if the time has not actually changed
                return;
            }

            //Calculate what "timeRange" we should be in, if we're not then switch and notify the world
            //Gonna do this by minutes
            var minuteOfDay = GameTime.Hour * 60f + GameTime.Minute;
            var expectedRange = (int) Math.Floor(minuteOfDay / timeBase.RangeInterval);

            if (expectedRange != sTimeRange)
            {
                sTimeRange = expectedRange;
                PacketSender.SendTimeToAll();
            }

            Hour = GameTime.ToString("%h");
            MilitaryHour = GameTime.ToString("HH");
            Minute = GameTime.ToString("mm");
            Second = GameTime.ToString("ss");
        }

        public static Color Color => TimeBase.GetTimeBase().DaylightHues[sTimeRange];

        public static int TimeRange => sTimeRange;
    }

}
