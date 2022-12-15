
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC
{
    [TestData(Value = @"2-4,6-8
2-3,4-5
5-7,7-9
2-8,3-7
6-6,4-6
2-6,4-8", Result = "2/4")]
    class Day4 : IDay
    {
        public string Calc(string input, bool test)
        {
            var lines = input.Split('\n').Where(line => line != "").Select(line => line.Split(',').Select(part => { var vals = part.Split('-').Select(str => int.Parse(str)); return new Range(vals.ElementAt(0), vals.ElementAt(1)); }));
            var result1 = lines.Count(ranges => { var r0 = ranges.ElementAt(0); var r1 = ranges.ElementAt(1); return r0.Contains(r1) || r1.Contains(r0); });
            var result2 = lines.Count(ranges => Range.Overlap(ranges.ElementAt(0), ranges.ElementAt(1)));
            return String.Format("{0}/{1}", result1, result2);
        }

        struct Range
        {
            int _low;
            int _high;
            public Range(int low, int high) { this._low = low; this._high = high; }
            public bool Contains(Range r) => _low <= r._low && _high >= r._high;
            public static bool Overlap(Range r1, Range r2) => r1._high >= r2._low && r1._low <= r2._high;
        };
    }
}
