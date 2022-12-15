
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC
{
    [TestData(Value = "mjqjpqmgbljsphdztnvjfqwrcgsmlb", Result = "7/19")]
    class Day6 : IDay
    {
        public string Calc(string input, bool test)
        {
            return String.Join('/', new int[] {4, 14}.Select(len => (len + Enumerable.Range(0, input.Length - len).First(index => input.Substring(index, len).Distinct().Count() == len)).ToString()));
        }
    }
}
