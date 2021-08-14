using System;
using System.Globalization;

using Intersect.Factories;
using Intersect.Network.Packets.Server;

namespace Intersect.Utilities
{
    public abstract class GameTime
    {
        private static GameTime _global;

        public static GameTime Global => _global ?? (_global = FactoryRegistry<GameTime>.Create(Timing.Global));

        private long _updateMs;

        public GameTime(Timing timing)
        {
            Timing = timing ?? throw new ArgumentNullException(nameof(timing));
            _updateMs = Timing.Milliseconds;
            Color = Color.Transparent;
            Rate = 1;
        }

        public virtual Color Color { get; protected set; }

        public virtual float Rate { get; protected set; }

        public virtual bool SyncTime => false;

        public virtual DateTime Time { get; protected set; }

        public Timing Timing { get; }

        public void Synchronize(TimePacket timePacket)
        {
            if (timePacket == default)
            {
                throw new ArgumentNullException(nameof(timePacket));
            }

            Color = timePacket.Color;
            Rate = timePacket.Rate;
            Time = timePacket.Time;
        }

        public virtual bool Update()
        {
            var lastTicks = Time.Ticks;
            var delta = Timing.Milliseconds - _updateMs;
            if (SyncTime)
            {
                Time = Timing.Now;
            }
            else if (delta > 0)
            {
                var seconds = delta / 1000;
                Time = Time.AddSeconds(seconds);
                _updateMs += 1000 * seconds;
            }

            return Time.Ticks != lastTicks;
        }

        public TimePacket ToPacket() => new TimePacket(Time, Rate, Color);

        /// <inheritdoc />
        public override string ToString() => ToString("h:mm:ss tt", DateTimeFormatInfo.CurrentInfo);

        public string ToString(string formatString) => ToString(formatString, DateTimeFormatInfo.CurrentInfo);

        public string ToString(string formatString, IFormatProvider formatProvider) => Time.ToString(formatString, formatProvider);

        public string ToString(IFormatProvider formatProvider) => Time.ToString(formatProvider);
    }
}
