
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
        public string Calc(String input)
        {
            return String.Join('/', new int[]{1, 3}.Select(n => input.Split('\n').Aggregate(new List<Tuple<int,int>>{new Tuple<int,int>(0,0)}, (acc,line) => acc.Concat(new Tuple<int,int>[]{(line.Trim() == "") ? new Tuple<int,int>(acc.Last().Item1 + 1, 0) : new Tuple<int,int>(acc.Last().Item1, acc.Last().Item2 + int.Parse(line))}).ToList()).GroupBy(t => t.Item1).Select(g => g.Last().Item2).OrderByDescending(i => i).Take(n).Sum().ToString()));
        }
    }
}
