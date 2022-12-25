
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AOC
{
    [TestData(Value = @"Sensor at x=2, y=18: closest beacon is at x=-2, y=15
Sensor at x=9, y=16: closest beacon is at x=10, y=16
Sensor at x=13, y=2: closest beacon is at x=15, y=3
Sensor at x=12, y=14: closest beacon is at x=10, y=16
Sensor at x=10, y=20: closest beacon is at x=10, y=16
Sensor at x=14, y=17: closest beacon is at x=10, y=16
Sensor at x=8, y=7: closest beacon is at x=2, y=10
Sensor at x=2, y=0: closest beacon is at x=2, y=10
Sensor at x=0, y=11: closest beacon is at x=2, y=10
Sensor at x=20, y=14: closest beacon is at x=25, y=17
Sensor at x=17, y=20: closest beacon is at x=21, y=22
Sensor at x=16, y=7: closest beacon is at x=15, y=3
Sensor at x=14, y=3: closest beacon is at x=15, y=3
Sensor at x=20, y=1: closest beacon is at x=15, y=3", Result = "26/56000011")]
    class Day15 : IDay
    {
        public string Calc(string input, bool test)
        {
            var sensors = 
                Regex.Matches(input, "Sensor at x=(-?\\d+), y=(-?\\d+): closest beacon is at x=(-?\\d+), y=(-?\\d+)")
                .Select(m => m.Groups.Values.Skip(1).Select(g => int.Parse(g.Value)).ToArray())
                .Select(a => (x:a[0], y:a[1], bx:a[2], by:a[3],d:Math.Abs(a[2] - a[0]) + Math.Abs(a[3] - a[1])))
                .ToArray();
            IEnumerable<int> RangeFromToInclusive(int from, int to) { return Enumerable.Range(from, 1 + to - from); }
            var answer1 = sensors.Select(s => (s.x,s.y,s.bx,s.by,s.d,l:s.d - Math.Abs(s.y - (test ? 10 : 2000000))))
                .Where(s => s.l > 0)
                .Select(s => 
                    RangeFromToInclusive(s.x - s.l, s.x + s.l)
                    .Where(x => s.by != (test ? 10 : 2000000) || s.bx != x))
                    .Aggregate(new List<int>(), (agg, x) => agg.Union(x).ToList()
                );
            var answer2 = sensors
                .Where(s => sensors.Any(s2 => Math.Abs(s2.x - s.x) + Math.Abs(s2.y - s.y) == 2 + s.d + s2.d))
                .Select(s => (s.x,s.y,d:s.d+1))
                .Select(s => 
                    RangeFromToInclusive(Math.Max(s.y - s.d, 0), Math.Min(s.y + s.d, (test) ? 20 : 4000000))
                    .Select(y => (x:s.d - Math.Abs(s.y - y), y:y))
                    .Select(p => new (int x, int y)[] { (x:s.x - p.x, y:p.y), (x:s.x + p.x, y:p.y)}.Where(p => p.x >= 0 && p.x <= ((test) ? 20 : 4000000)))
                    .SelectMany(p => p)
                )
                .SelectMany(p => p)
                .First(p => !sensors.Any(s => Math.Abs(s.x - p.x) + Math.Abs(s.y - p.y) <= s.d));
            return string.Format("{0}/{1}", answer1.Count(), 4000000 * (long)answer2.x + answer2.y);
        }
    }
}