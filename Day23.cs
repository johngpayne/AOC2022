
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
            var dirs = new (int x, int y, int bits)[] { (0, -1, 448), (0, 1, 7), (-1, 0, 292), (1, 0, 73) };
            var ret1 = 0;
            var ret2 = 0;
            while(true)
            {
                var elfMoves =
                    elves
                    .Select(elf => (elf, Enumerable.Range(elf.y - 1, 3).Select(y => Enumerable.Range(elf.x - 1, 3).Select(x => (x,y))).SelectMany(p => p).Aggregate(0, (agg, p) => agg * 2 + (elves.Contains(p) ? 1 : 0))))
                    .Select(p => p.Item2 == 16 ? (elf: p.Item1, x: 0, y: 0, done: true) : dirs.Where(dir => (p.Item2 & dir.bits) == 0).Select(dir => (elf: p.elf, x: dir.x, y: dir.y, done: false)).FirstOrDefault((elf: p.elf, x: 0, y: 0, done: false)))
                    .Select(p => (done: p.done, originalPos: p.elf, newPos: (x: p.elf.x + p.x, y: p.elf.y + p.y)))
                    .GroupBy(p => p.newPos)
                    .ToLookup(g => g.Count())
                    .Select(g => g.Select(c => c.Select(p => (done: p.done, pos: g.Key == 1 ? p.newPos : p.originalPos))).SelectMany(e => e))
                    .SelectMany(e => e);
                elves = elfMoves.Select(e => e.pos).ToHashSet();
                dirs = dirs.Skip(1).Concat(new (int x, int y, int bits)[] { dirs.First() }).ToArray();
                ret2++;
                if (ret2 == 10)
                {
                    ret1 = (1 + elves.Max(elf => elf.x) - elves.Min(elf => elf.x)) * (1 + elves.Max(elf => elf.y) - elves.Min(elf => elf.y)) - elves.Count();
                }
                if (elfMoves.Count(e => e.done) == elves.Count())
                {
                    break;
                }
            }
        
            return string.Format("{0}/{1}", ret1, ret2);
        }
    }
}
