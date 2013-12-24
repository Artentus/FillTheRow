using System;
using GameUtils;

namespace FillTheRow
{
    public class XorshiftEngine : Random, IEngineComponent
    {
        uint x, y, z, w;

        bool IEngineComponent.IsCompatibleTo(IEngineComponent component)
        {
            return !(component is Random);
        }

        object IEngineComponent.Tag
        {
            get { return null; }
        }

        public XorshiftEngine()
            : this(Environment.TickCount)
        { }

        public XorshiftEngine(int seed)
        {
            var rnd = new Random(seed);
            x = (uint)rnd.Next() + 1;
            y = (uint)rnd.Next() + 1;
            z = (uint)rnd.Next() + 1;
            w = (uint)rnd.Next() + 1;
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
