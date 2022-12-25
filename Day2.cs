
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC
{
    [TestData(Value = @"A Y
B X
C Z", Result = "15/12")]
    class Day2 : IDay
    {
        public string Calc(string input, bool test)
        {
            return 
                string.Join('/', 
                    Enumerable.Range(0, 2).Select(rule => 
                    {
                        var outcomes =
                            Enumerable.Range(0, 3)
                            .Select(otherPick => 
                                Enumerable.Range(0, 3).Select(offset => 
                                    (
                                        "ABC"[otherPick] + " " + "XYZ"[(rule == 1) ? (offset + 1) % 3 : (otherPick + offset) % 3], 
                                        1 + ((otherPick + offset) % 3) + 3 * ((offset + 1) % 3)
                                    )
                                )
                            )
                            .SelectMany(p => p)
                            .ToDictionary(p => p.Item1, p => p.Item2);
                        return 
                            input
                            .Split('\n')
                            .Select(line => line.Trim())
                            .Where(line => line != "")
                            .Sum(line =>
                                outcomes[line]
                            )
                            .ToString();
                    })
                );
        }
    }
}
