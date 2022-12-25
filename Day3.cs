
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
            var lines = 
                input
                .Split('\n')
                .Select(line => line.Trim())
                .Where(line => line != "");
            var scoreParts = (IEnumerable<IEnumerable<char>> parts) =>
                parts
                .ElementAt(0)
                .Where(ch => 
                    parts
                    .Skip(1)
                    .All(part => 
                        part.Contains(ch)
                    )
                )
                .Distinct()
                .Sum(c => Char.IsAsciiLetterLower(c) ? (1 + (c - 'a')) : (27 + (c - 'A'))); 
            return String.Format(
                "{0}/{1}", 
                lines.Select(line => scoreParts(line.Chunk(line.Length / 2))).Sum(), 
                lines.Chunk(3).Sum(chunk => scoreParts(chunk))
            );
        }
    }
}
