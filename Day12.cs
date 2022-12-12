
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
        public string Calc(string input)
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
                    var mapDirs = map.Select(row => row.Select(h => (x:0, y:0)).ToArray()).ToArray();
                    var opens = starts.Select(coord => (n:0, x:coord.x, y:coord.y)).ToArray();
                    while (true)
                    {
                        opens = opens.Select(
                            current => 
                                dirs.Select(
                                    dir =>
                                    {
                                        (int n, int x, int y) next = (current.n + 1, current.x + dir.x, current.y + dir.y);
                                        if (next.x >= 0 && next.x < map[0].Length && next.y >= 0 && next.y < map.Length && mapDirs[next.y][next.x] == (0,0) && map[next.y][next.x] <= map[current.y][current.x] + 1)
                                        {
                                            mapDirs[next.y][next.x] = (-dir.x, -dir.y);
                                            return next;
                                        }
                                        else
                                        {
                                            return (n:0, x:0, y:0);
                                        }
                                    }
                                )
                        ).SelectMany(t => t).Where(t => t.n > 0).ToArray();
                        int ret = opens.Where(t => t.x == exit.x && t.y == exit.y).FirstOrDefault((n:0,0,0)).n;
                        if (ret > 0)
                        {
                            return ret;
                        }
                    }
                };

            return string.Format("{0}/{1}", getShortest(new (int x, int y)[] { start }), getShortest(findAll(h => h == 0)));
        }
    }
}
