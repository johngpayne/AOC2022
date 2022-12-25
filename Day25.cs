
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AOC
{
    [TestData(Value = @"1=-0-2
12111
2=0=
21
2=01
111
20012
112
1=-1=
1-12
12
1=
122", Result = "2=-1=0")]
   class Day25 : IDay
    {
        public string Calc(string input, bool test)
        {
            string Encode(long num) { return (num == 0) ? "" : Encode((num + 2) / 5) + "=-012"[(int)((num + 2) % 5)]; }
            return Encode(
                input
                .Split('\n')
                .Select(line => line.Trim())
                .Where(line => line != "")
                .Select(line => line.Aggregate((long)0, (agg, digit) => 5 * agg + "=-012".IndexOf(digit) - 2))
                .Sum()
            );
        }
    }
}
 



