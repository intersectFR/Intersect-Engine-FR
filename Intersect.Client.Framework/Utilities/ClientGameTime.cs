using System.Linq;

using Intersect.Factories;
using Intersect.Utilities;

namespace Intersect.Client.Framework.Utilities
{
    /// <inheritdoc />
    public class ClientGameTime : GameTime
    {
        public new static ClientGameTime Global => GameTime.Global as ClientGameTime;

        private long _lastUpdateColorMs;

        private ColorF _tint;
        
        /// <inheritdoc />
        public ClientGameTime(Timing timing) : base(timing)
        {
            _lastUpdateColorMs = Timing.Milliseconds;
            _tint = ColorF.White;
        }

        public ColorF Tint => _tint.Clone();

        /// <inheritdoc />
        public override bool Update()
        {
            if (!base.Update())
            {
                return false;
            }

            var delta = Timing.Milliseconds - _lastUpdateColorMs;
            var lerpAmount = 255 * delta / 10000f;
            _tint.A = MathHelper.Lerp(_tint.A, Color.A, lerpAmount);
            _tint.R = MathHelper.Lerp(_tint.R, Color.R, lerpAmount);
            _tint.G = MathHelper.Lerp(_tint.G, Color.G, lerpAmount);
            _tint.B = MathHelper.Lerp(_tint.B, Color.B, lerpAmount);
            _lastUpdateColorMs = Timing.Global.Milliseconds;

            return true;
        }
    }

    /// <inheritdoc />
    public class ClientGameTimeFactory : IFactory<GameTime>
    {
        /// <inheritdoc />
        public GameTime Create(params object[] args)
        {
            var timing = Timing.Global;
            if (args.FirstOrDefault() is Timing argTiming)
            {
                timing = argTiming;
            }

            return new ClientGameTime(timing);
        }
    }
}
