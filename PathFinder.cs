using SpurRoguelike.Core.Primitives;
using System.Collections.Generic;

namespace SpurRoguelike.PlayerBot
{
    public static class PathFinder
    {
        public static Location[] BFS(Location start, Location finish, HashSet<Location> available)
        {
            if (start == finish)
                return null;
            var visited = new Dictionary<Location, Location>();
            var queue = new Queue<Location>();
            queue.Enqueue(start);
            visited[start] = default(Location);
            while (queue.Count != 0)
            {
                var current = queue.Dequeue();
                foreach (var loc in current.Neighbours())
                {
                    if (!visited.ContainsKey(loc))
                        if (available.Contains(loc))
                        {
                            visited[loc] = current;
                            queue.Enqueue(loc);
                            if (loc == finish)
                                return GetTrack(loc, visited);
                        }
                }

            }
            return null;
        }

        static Location[] GetTrack(Location finish, Dictionary<Location, Location> visited)
        {
            var track = new List<Location>();
            while (finish != default(Location))
            {
                track.Add(finish);
                finish = visited[finish];
            }
            track.Reverse();
            return track.ToArray();
        }
    }
}
