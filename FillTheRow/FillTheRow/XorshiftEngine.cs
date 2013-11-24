using System;

namespace FillTheRow
{
    public class XorshiftEngine : Random
    {
        uint x, y, z, w;

        public XorshiftEngine()
            : this(Environment.TickCount)
        { }

        public XorshiftEngine(int seed)
        {
            var rnd = new Random(seed);
            x = (uint)rnd.Next(1, int.MaxValue);
            y = (uint)rnd.Next(1, int.MaxValue);
            z = (uint)rnd.Next(1, int.MaxValue);
            w = (uint)rnd.Next(1, int.MaxValue);
            for (int i = 0; i < 10; i++)
                this.NextUInt();
        }

        private uint NextUInt()
        {
            uint t = (x ^ (x << 11));
            x = y; y = z; z = w;
            w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));
            return w - 1;
        }

        protected override double Sample()
        {
            return (double)this.NextUInt() / (double)uint.MaxValue;
        }
    }
}
