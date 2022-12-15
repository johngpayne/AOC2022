
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC
{
    [TestData(Value = @"vJrwpWtwJgWrhcsFMMfFFhFp
jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL
PmmdzqPrVvPwwTWBwg
wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn
ttgJtRGJQctTZtZT
CrZsJsPPZsGzwwsLwLmpwMDw", Result = "157/70")]
    class Day3 : IDay
    {
        public string Calc(string input, bool test)
        {
            var lines = input.Split('\n').Where(line => line.Trim() != "");
            var scoreChar = (char c) => { if (Char.IsAsciiLetterLower(c)) return (1 + (c - 'a')); else if (Char.IsAsciiLetterUpper(c)) return (27 + (c - 'A')); else return 0; };
            int scoreParts<T>(IEnumerable<T> parts) where T : IEnumerable<char> { return parts.ElementAt(0).Where(ch => parts.Skip(1).All(part => part.Contains(ch))).Distinct().Sum(ch => scoreChar(ch)); }
            return String.Format("{0}/{1}", lines.Select(line => scoreParts(line.Trim().Chunk(line.Length / 2))).Sum(), lines.Chunk(3).Sum(chunk => scoreParts(chunk)));
        }
    }
}
