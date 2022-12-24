
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AOC
{
    [TestData(Value = @"
#.######
#>>.<^<#
#.<..<<#
#>v.><>#
#<^v^^>#
######.#", Result = "18/54")]
   class Day24 : IDay
    {
        public string Calc(string input, bool test)
        {
            var dirLookup = new Dictionary<char,(int x, int y)> { {'>', (1, 0)}, {'<', (-1, 0)}, {'^', (0, -1)}, {'v', (0, 1)}};
            var map = 
                input
                .Split('\n')
                .Select(line => line.Trim())
                .Where(line => line != "")
                .Skip(1)
                .SkipLast(1)
                .Select((line, y) => 
                    line
                    .Skip(1)
                    .SkipLast(1)
                    .Select((ch, x) => 
                        (pos: (x: x, y: y), ch: ch)
                    )
                    .ToArray()
                ).ToArray();
            var blizzards = 
                map
                .SelectMany(p => p)
                .Where(p => p.ch != '.')
                .Select(p => (pos: p.pos, dir: dirLookup[p.ch]))
                .ToArray();
            
            int GCD(int a, int b) { while (b > 0) { (a,b) = (b, a % b); } return a; }
            int width = map[0].Length;
            int height = map.Length;
            var lcm = width * height / GCD(width, height);
            
            var maps = 
                Enumerable.Range(0, lcm)
                .Select(t =>
                    blizzards.Select(b =>
                        (x: (lcm * width + b.pos.x + t * b.dir.x) % width, y: (lcm * height + b.pos.y + t * b.dir.y) % height)
                    )
                    .ToHashSet()
                )
                .ToArray();

            int Route((int x, int y) startPos, (int x, int y) endPos, int timeOffset = 0)
            {
                var startTimes = 
                    Enumerable.Range(timeOffset + 1, lcm)
                    .Where(t => !maps[t % maps.Length].Contains(startPos));
                foreach(var startTime in startTimes)
                {
                    var attempts = new (int x, int y)[] { startPos };
                    var time = startTime;
                    while (true)
                    {
                        time++;
                        attempts = 
                            attempts
                            .Select(pos =>
                                dirLookup.Values
                                .Select(d => (x: pos.x + d.x, y: pos.y + d.y))
                            )
                            .SelectMany(p => p)
                            .Concat(attempts)
                            .Where(p => p.x >= 0 && p.y >= 0 && p.x < width && p.y < height && !maps[time % maps.Length].Contains(p))
                            .Distinct()
                            .ToArray();
                        if (attempts.Count() == 0)
                        {
                            break;
                        }
                        if (attempts.Contains(endPos))
                        {
                            return time + 1;
                        }
                    }
                }
                return -1;
            }

            var startPos = (x: 0, y: 0);
            var endPos = (x: width - 1, y: height - 1);
            return string.Format("{0}/{1}", Route(startPos, endPos), Route(startPos, endPos, Route(endPos, startPos, Route(startPos, endPos))));
        }
    }
}