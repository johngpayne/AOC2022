
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC
{
    [TestData(Value = @"    [D]    
[N] [C]    
[Z] [M] [P]
 1   2   3 

move 1 from 2 to 1
move 3 from 1 to 3
move 2 from 2 to 1
move 1 from 1 to 2", Result = "CMZ/MCD")]
    class Day5 : IDay
    {
        public string Calc(string input)
        {
            var lines = input.Split('\n');
            var lineNotEmpty = (string line) => line.Trim() != "";
            var linesInit = lines.TakeWhile(lineNotEmpty).SkipLast(1);
            var linesCommands = lines.SkipWhile(lineNotEmpty).Where(lineNotEmpty);
            var numStacks = linesInit.Max(line => (line.Length + 1) / 4);
            return string.Join('/', new Func<IEnumerable<char>,IEnumerable<char>>[] {a => a.Reverse(), a => a}.Select(
                f => 
                {
                    var stacks = Enumerable.Range(0, numStacks).Select(stackIndex => linesInit.Select(line => line.PadRight(numStacks * 4)[4 * stackIndex + 1]).SkipWhile(ch => ch == ' ').Reverse().ToList()).ToArray();
                    foreach (var command in linesCommands)
                    {
                        var vals = command.Split(' ').Where((x, i) => (i & 1) == 1).Select((x, i) => int.Parse(x) - ((i != 0) ? 1 : 0)).ToArray();
                        stacks[vals[2]].AddRange(f(stacks[vals[1]].TakeLast(vals[0])));
                        stacks[vals[1]].RemoveRange(stacks[vals[1]].Count() - vals[0], vals[0]);
                    }
                    return new String(stacks.Select(stack => stack.Last()).ToArray());
                }
            ));
        }
    }
}
