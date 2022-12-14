
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AOC
{
    [TestData(Value = @"498,4 -> 498,6 -> 496,6
503,4 -> 502,4 -> 502,9 -> 494,9", Result = "24/93")]
    class Day14 : IDay
    {
        public string Calc(string input)
        {
            var MinMaxRange = (int p0, int p1) => Enumerable.Range(Math.Min(p0, p1), 1 + Math.Abs(p0 - p1));
            var map = input.Split("\n").Select(line => line.Trim()).Where(line => line != "").Select(line => line.Split(" -> ").Select(coord => coord.Split(',').Select(s => int.Parse(s))).Select(a => (x:a.First(), y:a.Last()))).Select(strip => strip.Select((p, i) => Enumerable.Repeat(p, (i == 0 || i == strip.Count() - 1) ? 1 : 2)).SelectMany(a => a).Chunk(2).Select(p => MinMaxRange(p[0].x, p[1].x).Select(x => MinMaxRange(p[0].y, p[1].y).Select(y => (x:x,y:y))).SelectMany(c => c)).SelectMany(c => c)).SelectMany(c => c).Distinct().ToHashSet();
            int maxY = 1 + map.Max(p => p.y);
            var ys = Enumerable.Range(0, 2 * maxY * maxY).Select(i => { var pos = Enumerable.Range(0, 1 + maxY).Aggregate((x:500, y:0), (pos, i) => new (int x, int y)[] { (pos.x, pos.y + 1), (pos.x - 1, pos.y + 1), (pos.x + 1, pos.y + 1)}.FirstOrDefault(t => t.y <= maxY && !map.Contains(t), pos)); map.Add(pos); return pos.y; }).ToArray();
            return String.Format("{0}/{1}", Array.FindIndex(ys, y => y == maxY), 1 + Array.FindIndex(ys, y => y == 0));
        }
    }
}
  