
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AOC
{
    [TestData(Value = @"
....#..
..###.#
#...#.#
.#...##
#.###..
##.#.##
.#..#..", Result = "110/20")]
   class Day23 : IDay
    {
        public string Calc(string input, bool test)
        {
            var elves = input.Split('\n').Select(line => line.Trim()).Where(line => line != "").Select((line, y) => line.Select((ch, x) => (x, y)).Where(p => line[p.x] == '#')).SelectMany(p => p).ToHashSet();
            var dirs = new (int x, int y, int bits)[] { (0, -1, 224), (0, 1, 7), (-1, 0, 148), (1, 0, 41) };
            var ret1 = 0;
            var ret2 = 0;
            var offsets = Enumerable.Range(-1, 3).Select(y => Enumerable.Range(-1, 3).Select(x => (x,y))).SelectMany(p => p).Where(p => p.x != 0 || p.y != 0).ToArray();
            var numElves = elves.Count();
            while(true)
            {
                var elfMoves =
                    elves
                    .Select(elf => (elf, offsets.Aggregate(0, (agg, p) => agg * 2 + (elves.Contains((p.x + elf.x, p.y + elf.y)) ? 1 : 0))))
                    .Select(p => p.Item2 == 0 ? (elf: p.Item1, xo: 0, yo: 0, stopped: true) : dirs.Where(dir => (p.Item2 & dir.bits) == 0).Select(dir => (elf: p.elf, xo: dir.x, yo: dir.y, stopped: false)).FirstOrDefault((elf: p.elf, xo: 0, yo: 0, stopped: false)))
                    .Select(p => (stopped: p.stopped, originalPos: p.elf, newPos: (x: p.elf.x + p.xo, y: p.elf.y + p.yo)))
                    .GroupBy(p => p.newPos)
                    .ToLookup(g => g.Count())
                    .Select(g => g.Select(c => c.Select(p => (stopped: p.stopped, pos: g.Key == 1 ? p.newPos : p.originalPos))).SelectMany(e => e))
                    .SelectMany(e => e)
                    .ToArray();
                elves = elfMoves.Select(e => e.pos).ToHashSet();
                dirs = dirs.Skip(1).Concat(dirs).Take(4).ToArray();
                ret2++;
                if (ret2 == 10)
                {
                    ret1 = (1 + elves.Max(elf => elf.x) - elves.Min(elf => elf.x)) * (1 + elves.Max(elf => elf.y) - elves.Min(elf => elf.y)) - numElves;
                }
                if (elfMoves.Count(e => e.stopped) == numElves)
                {
                    break;
                }
            }
        
            return string.Format("{0}/{1}", ret1, ret2);
        }
    }
}
