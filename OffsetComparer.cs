using SpurRoguelike.Core.Primitives;
using System;
using System.Collections.Generic;

namespace SpurRoguelike.PlayerBot
{
    class OffsetComparer : IComparer<Offset>
    {
        public int Compare(Offset x, Offset y)
        {
            return DistanceTo(x).CompareTo(DistanceTo(y));
        }

        private static int DistanceTo(Offset offset) => Math.Abs(offset.XOffset) + Math.Abs(offset.YOffset);

    }
}
