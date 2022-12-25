
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
            var lines = 
                input
                .Split('\n')
                .Select(line => line.Trim())
                .Where(line => line != "")
                .Select(line => 
                    line
                    .Split(',')
                    .Select(part => 
                        part
                        .Split('-')
                        .Select(str => int.Parse(str)) 
                        .ToArray()
                    )
                    .ToArray()
                )
                .ToArray();
            var result1 = lines.Count(r => (r[0][0] <= r[1][0] && r[0][1] >= r[1][1]) || (r[1][0] <= r[0][0] && r[1][1] >= r[0][1]));
            var result2 = lines.Count(r => r[0][1] >= r[1][0] && r[0][0] <= r[1][1]);
            return String.Format("{0}/{1}", result1, result2);
        }
    }
}
