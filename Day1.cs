
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC
{
    [TestData(Value=@"1000
2000
3000

4000

5000
6000

7000
8000
9000

10000", Result = "24000/45000")]
    class Day1 : IDay
    {
        public string Calc(String input, bool test)
        {
            return 
                String.Join(
                    '/', 
                    new int[]{1, 3}.Select(n => 
                        input
                        .Split('\n')
                        .Aggregate(new List<(int, int)>{(0,0)}, (agg,line) => 
                            agg
                            .Concat(Enumerable.Repeat(line.Trim() == "" ? (agg.Last().Item1 + 1, 0) : (agg.Last().Item1, agg.Last().Item2 + int.Parse(line)), 1))
                            .ToList()
                        )
                        .GroupBy(t => t.Item1)
                        .Select(g => g.Last().Item2)
                        .OrderByDescending(i => i)
                        .Take(n)
                        .Sum()
                        .ToString()
                    )
                );
        }
    }
}
