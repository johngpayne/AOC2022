
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
                    .Select(elf => (originalPos: elf, area: offsets.Aggregate(0, (agg, p) => agg * 2 + (elves.Contains((p.x + elf.x, p.y + elf.y)) ? 1 : 0))))
                    .Select(elf => (originalPos: elf.originalPos, dir: (elf.area == 0) ? (x: 0, y: 0, bits: 0) : dirs.Where(dir => (elf.area & dir.bits) == 0).FirstOrDefault((x: 0, y: 0, bits: 0)), stopped: elf.area == 0))
                    .Select(elf => (originalPos: elf.originalPos, newPos: elf.stopped ? elf.originalPos : (x: elf.originalPos.x + elf.dir.x, y: elf.originalPos.y + elf.dir.y), stopped: elf.stopped))
                    .ToArray();

                elves = 
                    elfMoves
                    .Where(elf => elf.newPos != elf.originalPos)
                    .GroupBy(elf => elf.newPos)
                    .ToLookup(group => group.Count())
                    .Select(group => 
                        group
                        .SelectMany(elf => elf)
                        .Select(elf => group.Key == 1 ? elf.newPos : elf.originalPos)
                    )
                    .SelectMany(elf => elf)
                    .Concat(
                        elfMoves
                        .Where(elf => elf.newPos == elf.originalPos)
                        .Select(elf => elf.originalPos)
                    )
                    .ToHashSet();

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
