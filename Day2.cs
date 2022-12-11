
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
        public string Calc(string input)
        {
            return string.Join('/', Enumerable.Range(0, 2).Select(i => 
            {
                var outcomes = new Dictionary<string,int>(Enumerable.Range(0, 3).Select(otherPick => Enumerable.Range(0, 3).Select(offset => new KeyValuePair<string,int>("ABC"[otherPick] + " " + "XYZ"[(i == 1) ? (offset + 1) % 3 : (otherPick + offset) % 3], 1 + ((otherPick + offset) % 3) + 3 * ((offset + 1) % 3)))).SelectMany(e => e));
                return input.Split('\n').Sum(line => outcomes.GetValueOrDefault(line.Trim())).ToString();
            }));
        }
    }
}
