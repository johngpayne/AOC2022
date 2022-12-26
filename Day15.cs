
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
            IEnumerable<int> RangeFromToInclusive(int from, int to) { return Enumerable.Range(from, 1 + to - from); }
        
            var sensors = 
                Regex.Matches(input, "Sensor at x=(-?\\d+), y=(-?\\d+): closest beacon is at x=(-?\\d+), y=(-?\\d+)")
                .Select(match => 
                    match.Groups.Values
                    .Skip(1)
                    .Select(value => 
                        int.Parse(value.Value)
                    )
                    .ToArray()
                )
                .Select(a => (x: a[0], y: a[1], bx: a[2], by: a[3], dist: Math.Abs(a[2] - a[0]) + Math.Abs(a[3] - a[1])))
                .ToArray();

            var testRow = test ? 10 : 2000000;
            var answer1 = 
                sensors
                .Select(s => (s.x, s.y, s.bx, s.by, s.dist, halfWidthAtTest: s.dist - Math.Abs(s.y - testRow)))
                .Where(s => s.halfWidthAtTest > 0)
                .Select(s => 
                    RangeFromToInclusive(s.x - s.halfWidthAtTest, s.x + s.halfWidthAtTest)
                    .Where(x => s.by != testRow || s.bx != x)
                )
                .Aggregate(new List<int>(), (agg, sensorRange) => agg.Union(sensorRange).ToList())
                .Count();

            var testMax = test ? 20 : 4000000;
            var answer2 = sensors
                .Where(s1 => sensors.Any(s2 => Math.Abs(s2.x - s1.x) + Math.Abs(s2.y - s1.y) == 2 + s1.dist + s2.dist))
                .Select(s => 
                    RangeFromToInclusive(Math.Max(s.y - (s.dist + 1), 0), Math.Min(s.y + (s.dist + 1), testMax))
                    .Select(y => 
                    { 
                        var halfWidth = (s.dist + 1) - Math.Abs(s.y - y); 
                        return new (int x, int y)[] { (s.x - halfWidth, y), (s.x + halfWidth, y) };
                    })
                )
                .SelectMany(p => p)
                .SelectMany(p => p)
                .Select(p => (pos: p, value: 4000000 * (long)p.x + p.y))
                .First(p => !sensors.Any(s => Math.Abs(s.x - p.pos.x) + Math.Abs(s.y - p.pos.y) <= s.dist))
                .value;
                
            return string.Format("{0}/{1}", answer1, answer2);
        }
    }
}