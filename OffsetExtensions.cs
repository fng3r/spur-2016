using SpurRoguelike.Core.Primitives;
using System;

namespace SpurRoguelike.PlayerBot
{
    public static class OffsetExtensions
    {
        public static Offset AndvancedSnapToStep(this Offset offset)
        {
            var normalized = offset.Normalize();

            if (normalized.XOffset == 0 || normalized.YOffset == 0)
                return offset;

            return Math.Abs(offset.XOffset) >= Math.Abs(offset.YOffset) ? new Offset(normalized.XOffset, 0) : new Offset(0, normalized.YOffset);
        }

        public static Offset SnapToHorizontalStep(this Offset offset)
        {
            return new Offset(Math.Abs(offset.XOffset), 0);
        }

        public static Offset SnapToVerticalStep(this Offset offset)
        {
            return new Offset(0, Math.Abs(offset.YOffset));
        }
    }
}
