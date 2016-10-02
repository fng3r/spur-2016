using SpurRoguelike.Core.Primitives;
using System;
using System.Collections.Generic;

namespace SpurRoguelike.PlayerBot
{

    public static class LocationExtensions
    {
        public static IEnumerable<Location> Neighbours(this Location location)
        {
            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (Math.Abs(dx) + Math.Abs(dy) == 1)
                        yield return new Location(location.X + dx, location.Y + dy);
                }
        }
    }
}
