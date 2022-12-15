
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC
{
    [TestData(Value = @"Sabqponm
abcryxxl
accszExk
acctuvwj
abdefghi", Result = "31/29")]
    class Day12 : IDay
    {
        public string Calc(string input, bool test)
        {
            var lines = input.Split("\n").Select(line => line.Trim()).Where(line => line != "");
            var map  = lines.Select(line => line.Select(ch => ch - 'a').ToArray()).ToArray();
            
            var allMapPos = Enumerable.Range(0, map.Length).Select(y => Enumerable.Range(0, map[y].Length).Select(x => (x, y))).SelectMany(p => p).ToArray();
            var findAll = (Func<int,bool> f) => allMapPos.Where(p => f(map[p.y][p.x]));
            var find = (char ch) => findAll(h => h == (ch - 'a')).First();

            var start = find('S'); map[start.y][start.x] = 0; 
            var exit = find('E'); map[exit.y][exit.x] = 25;

            var getShortest = (IEnumerable<(int x, int y)> starts) =>
                {
                    var dirs = new (int x, int y)[] { (1, 0), (-1, 0), (0, 1), (0, -1)};
                    var mapDirs = map.Select(row => row.Select(h => (n:0, x:0, y:0)).ToArray()).ToArray();
                    var opens = starts.Select(coord => (n:0, x:coord.x, y:coord.y, dir:(x:0, y:0))).ToArray();
                    while (true)
                    {
                        opens = opens.Select(
                            current => 
                                dirs
                                .Select(dir => (n:current.n + 1, x:current.x + dir.x, y:current.y + dir.y, dir:dir))
                                .Where(next => next.x >= 0 && next.x < map[0].Length && next.y >= 0 && next.y < map.Length && mapDirs[next.y][next.x].n == 0 && map[next.y][next.x] <= map[current.y][current.x] + 1)
                                .Select(next =>
                                {
                                    mapDirs[next.y][next.x] = (next.n, -next.dir.x, -next.dir.y);
                                    return next;
                                })
                        ).SelectMany(t => t).ToArray();
                        if (mapDirs[exit.y][exit.x].n > 0) return mapDirs[exit.y][exit.x].n;
                    }
                };

            return string.Format("{0}/{1}", getShortest(new (int x, int y)[] { start }), getShortest(findAll(h => h == 0)));
        }
    }
}
