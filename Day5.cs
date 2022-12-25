
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
        public string Calc(string input, bool test)
        {
            var lines = 
                input
                .Split('\n')
                .Select(line => line.TrimEnd())
                .Where(line => line != "")
                .ToLookup(line => line.TrimStart()[0] == '[');
            
            var numStacks = lines[true].Max(line => (line.Length + 1) / 4);
            var startStacks = Enumerable.Range(0, numStacks)
                .Select(stackIndex => 
                    lines[true].Select(line => 
                        line
                        .PadRight(numStacks * 4)[4 * stackIndex + 1])
                        .SkipWhile(ch => ch == ' ')
                        .Reverse()
                        .ToArray()
                    )
                .ToArray();
                
            return string.Join('/', 
                Enumerable.Range(0, 2).Select(puzzle => 
                    new String( 
                        lines[false]
                        .Skip(1)
                        .Select(command => Regex.Match(command, "move (\\d+) from (\\d+) to (\\d+)").Groups.Values.Skip(1).Select((v,i) => int.Parse(v.Value) - (i != 0 ? 1 : 0)).ToArray())
                        .Aggregate(startStacks, (stacks, vals) =>
                            stacks.Select((stack, i) =>
                                (i == vals[2]) ? stack.Concat(((Func<IEnumerable<char>,IEnumerable<char>>)((s) => puzzle == 0 ? s.Reverse() : s))(stacks[vals[1]].TakeLast(vals[0]))).ToArray() :
                                (i == vals[1]) ? stack.SkipLast(vals[0]).ToArray() :
                                stack
                            )
                            .ToArray()
                        )
                        .Select(stack => stack.Last())
                        .ToArray()
                    )
                )
            );
        }
    }
}
